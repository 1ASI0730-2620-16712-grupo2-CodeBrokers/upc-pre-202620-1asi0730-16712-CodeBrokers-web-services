using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Shared.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Generic base repository providing CRUD operations for all entity types.
/// </summary>
/// <remarks>
///     Implements <see cref="IBaseRepository{TEntity, TId}"/> against the shared <see cref="AppDbContext"/>.
///     Derived repositories extend this class to add entity-specific queries.
///     It only registers changes; they are committed through <see cref="IUnitOfWork"/>.
/// </remarks>
/// <typeparam name="TEntity">The entity type managed by this repository.</typeparam>
/// <typeparam name="TId">The identifier type of the entity.</typeparam>
/// <param name="context">The EF Core database context.</param>
public class BaseRepository<TEntity, TId>(AppDbContext context) : IBaseRepository<TEntity, TId>
    where TEntity : class
    where TId : notnull
{
    /// <summary>
    ///     The EF Core <see cref="AppDbContext"/> instance available to derived repositories.
    /// </summary>
    protected readonly AppDbContext Context = context;

    /// <inheritdoc />
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    /// <inheritdoc />
    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().ToListAsync(cancellationToken);
    }
}
