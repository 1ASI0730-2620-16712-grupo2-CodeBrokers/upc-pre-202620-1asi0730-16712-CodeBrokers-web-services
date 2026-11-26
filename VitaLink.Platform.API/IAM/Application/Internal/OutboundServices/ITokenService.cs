using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;

/// <summary>
///     Port for issuing access tokens.
/// </summary>
/// <remarks>
///     The application layer depends only on this interface; the token format (JWT) lives in infrastructure.
/// </remarks>
public interface ITokenService
{
    /// <summary>
    ///     Issues a signed token for a user.
    /// </summary>
    /// <param name="user">User the token is issued to.</param>
    AccessToken Generate(User user);
}
