using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;

/// <summary>
///     Repository interface for monitoring rules.
/// </summary>
public interface IMonitoringRuleRepository : IBaseRepository<MonitoringRule, MonitoringRuleId>
{
    Task<IEnumerable<MonitoringRule>> FindByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default);
    Task<MonitoringRule?> FindByOlderAdultAndTypeAsync(Guid olderAdultId, string recordTypeCode, CancellationToken cancellationToken = default);
}