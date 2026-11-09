using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;

/// <summary>
///     Assembler to transform CreateMonitoringRuleResource to CreateMonitoringRuleCommand.
/// </summary>
public static class CreateMonitoringRuleCommandFromResourceAssembler
{
    public static CreateMonitoringRuleCommand ToCommandFromResource(CreateMonitoringRuleResource resource)
    {
        return new CreateMonitoringRuleCommand(
            resource.OlderAdultId.GetValueOrDefault(),
            resource.RecordTypeCode!,
            resource.MinThreshold,
            resource.MaxThreshold,
            Enum.Parse<SeverityLevel>(resource.Severity!, ignoreCase: true));
    }
}