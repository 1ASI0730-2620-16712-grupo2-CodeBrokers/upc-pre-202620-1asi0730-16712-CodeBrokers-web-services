using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Model.Queries;
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
    public async Task<AlertPage> FindPageAsync(GetAlertsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Page < 1 || query.PageSize is < 1 or > 100 ||
            (long)(query.Page - 1) * query.PageSize > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(query), "Invalid pagination.");

        var alerts = Context.Set<Alert>().AsNoTracking().AsQueryable();
        if (query.Status.HasValue)
            alerts = alerts.Where(a => a.Status == query.Status.Value);
        if (query.OlderAdultId.HasValue)
            alerts = alerts.Where(a => a.OlderAdultId == query.OlderAdultId.Value);

        var total = await alerts.CountAsync(cancellationToken);
        var items = await alerts.OrderByDescending(a => a.RaisedAt).ThenBy(a => a.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .ToListAsync(cancellationToken);
        return new AlertPage(items, total, query.Page, query.PageSize);
    }

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
