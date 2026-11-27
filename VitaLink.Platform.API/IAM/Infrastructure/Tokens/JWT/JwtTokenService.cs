using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;
using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.IAM.Infrastructure.Tokens.JWT;

/// <summary>
///     JWT implementation of <see cref="ITokenService" /> (HMAC SHA-256).
/// </summary>
/// <remarks>
///     Revoked tokens are kept in memory until they expire. This covers a single API instance; with several
///     instances the revocation list would have to move to a shared store (database or cache).
/// </remarks>
/// <param name="options">JWT settings (issuer, audience, signing key and expiration).</param>
/// <param name="revokedTokens">In-memory cache of revoked token identifiers.</param>
public class JwtTokenService(IOptions<JwtOptions> options, IMemoryCache revokedTokens) : ITokenService
{
    private const string RevokedPrefix = "iam:revoked-token:";

    private readonly JwtOptions _options = options.Value;

    private SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(_options.SigningKey!));

    /// <inheritdoc />
    public AccessToken Generate(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
                new Claim("role", user.Role.ToString().ToSnakeCase().ToUpperInvariant())
            ]),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }

    /// <inheritdoc />
    public async Task<ValidatedToken?> ValidateAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var result = await new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey
        });

        if (!result.IsValid || result.SecurityToken is not JsonWebToken jwt) return null;
        if (!Guid.TryParse(jwt.Subject, out var userId) || userId == Guid.Empty) return null;
        if (string.IsNullOrWhiteSpace(jwt.Id) || revokedTokens.TryGetValue(RevokedPrefix + jwt.Id, out _)) return null;

        return new ValidatedToken(new UserId(userId), jwt.Id, new DateTimeOffset(jwt.ValidTo, TimeSpan.Zero));
    }

    /// <inheritdoc />
    public void Revoke(string tokenId, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(tokenId)) return;
        // Kept a little longer than the token itself to cover the allowed clock skew.
        revokedTokens.Set(RevokedPrefix + tokenId, true, expiresAt.AddMinutes(1));
    }
}
