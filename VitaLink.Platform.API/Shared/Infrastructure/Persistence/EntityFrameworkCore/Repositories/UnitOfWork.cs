using VitaLink.Platform.API.Shared.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Unit of work implementation that coordinates the persistence of pending changes.
/// </summary>
/// <remarks>
///     Wraps EF Core's <c>SaveChangesAsync</c>: every change registered by the repositories
///     during a use case (add, update, remove) is persisted in a single atomic operation.
/// </remarks>
/// <param name="context">The EF Core database context.</param>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
