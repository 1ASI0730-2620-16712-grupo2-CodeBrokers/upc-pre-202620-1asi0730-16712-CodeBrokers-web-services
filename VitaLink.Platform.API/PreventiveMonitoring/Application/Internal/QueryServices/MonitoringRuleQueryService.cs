using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;

/// <summary>
///     Query service implementation for monitoring rules.
/// </summary>
public class MonitoringRuleQueryService : IMonitoringRuleQueryService
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;

    public MonitoringRuleQueryService(IMonitoringRuleRepository monitoringRuleRepository)
    {
        _monitoringRuleRepository = monitoringRuleRepository;
    }

    public async Task<IEnumerable<MonitoringRule>> Handle(Guid olderAdultId, CancellationToken cancellationToken = default)
    {
        return await _monitoringRuleRepository.FindByOlderAdultAsync(olderAdultId, cancellationToken);
    }

    public async Task<MonitoringRule?> Handle(Guid olderAdultId, string recordTypeCode, CancellationToken cancellationToken = default)
    {
        return await _monitoringRuleRepository.FindByOlderAdultAndTypeAsync(olderAdultId, recordTypeCode, cancellationToken);
    }
}