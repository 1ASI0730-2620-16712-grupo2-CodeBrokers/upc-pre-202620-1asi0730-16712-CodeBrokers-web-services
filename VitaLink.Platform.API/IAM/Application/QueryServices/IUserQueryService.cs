using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.Queries;

namespace VitaLink.Platform.API.IAM.Application.QueryServices;

/// <summary>
///     Read operations of the IAM bounded context.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    ///     Gets a user by identifier.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user, or <c>null</c> when it does not exist.</returns>
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);
}
