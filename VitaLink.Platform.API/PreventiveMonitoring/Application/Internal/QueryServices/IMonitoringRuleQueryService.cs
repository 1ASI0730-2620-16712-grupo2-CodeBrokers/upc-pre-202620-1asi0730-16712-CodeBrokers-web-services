using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.QueryServices;

/// <summary>
///     Query service interface for monitoring rules.
/// </summary>
public interface IMonitoringRuleQueryService
{
    Task<IEnumerable<MonitoringRule>> Handle(Guid olderAdultId);
    Task<MonitoringRule?> Handle(Guid olderAdultId, string recordTypeCode);
}