using VitaLink.Platform.API.IAM.Application.CommandServices;
using VitaLink.Platform.API.IAM.Application.Internal.OutboundServices;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;

namespace VitaLink.Platform.API.IAM.Application.Internal.CommandServices;

/// <summary>
///     Implementation of <see cref="ISessionCommandService" />.
/// </summary>
/// <param name="tokenService">Service that issues and revokes the access tokens.</param>
public class SessionCommandService(ITokenService tokenService) : ISessionCommandService
{
    /// <inheritdoc />
    public void Handle(SignOutCommand command) => tokenService.Revoke(command.TokenId, command.ExpiresAt);
}
