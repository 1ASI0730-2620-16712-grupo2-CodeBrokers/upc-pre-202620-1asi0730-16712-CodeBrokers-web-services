using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Builds the sign-in response.
/// </summary>
public static class AuthenticatedUserResourceFromResultAssembler
{
    /// <param name="result">Signed-in user and token.</param>
    public static AuthenticatedUserResource ToResourceFromResult(AuthenticatedUser result) =>
        new(result.User.Id.Value,
            result.User.Email.Value,
            UserRoleText.ToText(result.User.Role),
            result.Token.Value,
            result.Token.ExpiresAt);
}
