using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core implementation of <see cref="IAlertRepository" />.
/// </summary>
public class AlertRepository(AppDbContext context)
    : BaseRepository<Alert, AlertId>(context), IAlertRepository
{
    /// <inheritdoc />
    public async Task<int> CountPendingByCareProviderAsync(
        Guid careProviderId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Alert>()
            .CountAsync(a => a.AssignedProviderId == careProviderId && a.Status == AlertStatus.Pending,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Alert>> FindByCareProviderOrderBySeverityAsync(
        Guid careProviderId, AlertStatus? status, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Alert>().Where(a => a.AssignedProviderId == careProviderId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query
            .OrderBy(a => a.Severity == SeverityLevel.High ? 0 : a.Severity == SeverityLevel.Medium ? 1 : 2)
            .ThenByDescending(a => a.RaisedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Alert>> FindByOlderAdultAsync(
        Guid olderAdultId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Alert>()
            .Where(a => a.OlderAdultId == olderAdultId)
            .OrderByDescending(a => a.RaisedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Alert?> FindByHealthRecordAsync(
        Guid healthRecordId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Alert>()
            .FirstOrDefaultAsync(a => a.HealthRecordId == healthRecordId, cancellationToken);
    }
}