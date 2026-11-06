using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;

/// <summary>
///     Repository interface for HealthRecord aggregate.
/// </summary>
public interface IHealthRecordRepository
{
    Task AddAsync(HealthRecord record);
    Task<IEnumerable<HealthRecord>> FindByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default);
    Task<HealthRecord?> FindLatestByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default);
}