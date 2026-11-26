using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.IAM.Application.CommandServices;

/// <summary>
///     Handles the commands that create or change user accounts.
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    ///     Registers a new account. It is created active and already verified.
    /// </summary>
    /// <param name="command">Sign-up data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user, or the reason the sign-up failed.</returns>
    Task<Result<User, SignUpError>> Handle(SignUpCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Signs a user in and issues an access token. Only active, verified accounts can sign in.
    /// </summary>
    /// <param name="command">Sign-in data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user with their token, or the reason the sign-in failed.</returns>
    Task<Result<AuthenticatedUser, SignInError>> Handle(SignInCommand command, CancellationToken cancellationToken = default);
}
