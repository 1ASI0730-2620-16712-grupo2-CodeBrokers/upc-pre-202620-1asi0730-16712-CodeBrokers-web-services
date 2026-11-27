using VitaLink.Platform.API.IAM.Domain.Model.Commands;

namespace VitaLink.Platform.API.IAM.Application.CommandServices;

/// <summary>
///     Session operations of the IAM bounded context.
/// </summary>
public interface ISessionCommandService
{
    /// <summary>
    ///     Signs out: the access token of the request can no longer be used (US-25 CA-04).
    /// </summary>
    /// <param name="command">The sign-out command.</param>
    void Handle(SignOutCommand command);
}
