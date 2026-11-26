using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;
using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.IAM.Infrastructure.Tokens.JWT;

/// <summary>
///     Issues JWT access tokens signed with HMAC-SHA256.
/// </summary>
/// <remarks>
///     Claims: <c>sub</c> (user id), <c>email</c> and <c>role</c> (<c>CARE_PROVIDER</c>, <c>FAMILY_CAREGIVER</c>
///     or <c>OLDER_ADULT</c>).
/// </remarks>
/// <param name="options">Token settings.</param>
public class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    /// <inheritdoc />
    public AccessToken Generate(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpirationMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey!));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
                new Claim("role", user.Role.ToString().ToSnakeCase().ToUpperInvariant())
            ]),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken(new JsonWebTokenHandler().CreateToken(descriptor), expiresAt);
    }
}
