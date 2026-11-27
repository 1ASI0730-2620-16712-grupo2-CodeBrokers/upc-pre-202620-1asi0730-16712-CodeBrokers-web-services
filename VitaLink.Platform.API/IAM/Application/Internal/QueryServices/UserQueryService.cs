using VitaLink.Platform.API.IAM.Application.QueryServices;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.Queries;
using VitaLink.Platform.API.IAM.Domain.Repositories;

namespace VitaLink.Platform.API.IAM.Application.Internal.QueryServices;

/// <summary>
///     Implementation of <see cref="IUserQueryService" /> backed by the user repository.
/// </summary>
/// <param name="userRepository">User repository.</param>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    /// <inheritdoc />
    public Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default) =>
        userRepository.FindByIdAsync(query.UserId, cancellationToken);
}
