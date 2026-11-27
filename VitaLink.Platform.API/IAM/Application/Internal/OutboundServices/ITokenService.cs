using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;

/// <summary>
///     Issues, validates and revokes the access tokens of the users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     Issues an access token for the user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>The token and its expiration.</returns>
    AccessToken Generate(User user);

    /// <summary>
    ///     Validates the signature, issuer, audience and expiration of a token, and that it was not revoked.
    /// </summary>
    /// <param name="token">The raw token sent in the Authorization header.</param>
    /// <returns>The token data, or <c>null</c> when the token is not valid.</returns>
    Task<ValidatedToken?> ValidateAsync(string token);

    /// <summary>
    ///     Revokes a token until it expires, so it cannot be used again (sign-out).
    /// </summary>
    /// <param name="tokenId">The <c>jti</c> of the token.</param>
    /// <param name="expiresAt">When the token expires; after that it is rejected anyway.</param>
    void Revoke(string tokenId, DateTimeOffset expiresAt);
}
