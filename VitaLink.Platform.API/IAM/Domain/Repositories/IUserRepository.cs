using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.IAM.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="User"/> aggregate.
/// </summary>
public interface IUserRepository : IBaseRepository<User, UserId>
{
    /// <summary>
    ///     Finds a user by email.
    /// </summary>
    /// <param name="email">Email used to sign in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user, or <c>null</c> when no account uses that email.</returns>
    Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks whether an account already uses an email.
    /// </summary>
    /// <param name="email">Email to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
