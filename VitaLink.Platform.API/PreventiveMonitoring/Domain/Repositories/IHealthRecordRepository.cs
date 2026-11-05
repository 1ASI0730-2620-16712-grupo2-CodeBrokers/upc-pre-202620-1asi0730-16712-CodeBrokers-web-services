using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;

/// <summary>
///     Repository interface for health records.
/// </summary>
public interface IHealthRecordRepository : IBaseRepository<HealthRecord, HealthRecordId>
{
    Task<IEnumerable<HealthRecord>> FindByOlderAdultAsync(
        Guid olderAdultId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        string? recordTypeCode,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<HealthRecord>> FindLatestByOlderAdultAsync(
        Guid olderAdultId,
        int take,
        CancellationToken cancellationToken = default);
}