using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;

/// <summary>
///     Assembler to transform MonitoringRule aggregate entity to MonitoringRuleResource.
/// </summary>
public static class MonitoringRuleResourceFromEntityAssembler
{
    public static MonitoringRuleResource ToResourceFromEntity(MonitoringRule entity)
    {
        return new MonitoringRuleResource(
            entity.Id.Value,
            entity.OlderAdultId,
            entity.RecordTypeCode,
            entity.Threshold.Min,
            entity.Threshold.Max,
            entity.SeverityLevel,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt);
    }
}