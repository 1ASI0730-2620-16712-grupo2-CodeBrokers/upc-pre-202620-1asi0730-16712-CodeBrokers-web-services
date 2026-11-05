using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.IAM.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core implementation of <see cref="IUserRepository"/>.
/// </summary>
/// <remarks>
///     Common operations come from <see cref="BaseRepository{TEntity, TId}"/>.
///     It only registers changes; they are committed through the Unit of Work.
/// </remarks>
/// <param name="context">The EF Core database context.</param>
public class UserRepository(AppDbContext context)
    : BaseRepository<User, UserId>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}
