namespace VitaLink.Platform.API.Shared.Domain.Repositories;

/// <summary>
///     Unit of work interface.
/// </summary>
/// <remarks>
///     Coordinates the changes registered by the repositories during a use case
///     and commits them to the database in a single operation.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    ///     Commits all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous commit operation.</returns>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
