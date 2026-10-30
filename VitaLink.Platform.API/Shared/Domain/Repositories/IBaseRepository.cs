namespace VitaLink.Platform.API.Shared.Domain.Repositories;

/// <summary>
///     Base repository interface for all repositories.
/// </summary>
/// <remarks>
///     Defines the basic CRUD operations shared by every repository. Entity identifiers are
///     <see cref="Guid"/> values, matching the UUID primary keys defined in the database design.
///     Repositories do not save changes; changes are committed through <see cref="IUnitOfWork"/>.
/// </remarks>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IBaseRepository<TEntity>
{
    /// <summary>
    ///     Adds an entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Marks an entity as updated.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    ///     Marks an entity as removed.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    ///     Lists all entities.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An enumerable containing all entities.</returns>
    Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default);
}
