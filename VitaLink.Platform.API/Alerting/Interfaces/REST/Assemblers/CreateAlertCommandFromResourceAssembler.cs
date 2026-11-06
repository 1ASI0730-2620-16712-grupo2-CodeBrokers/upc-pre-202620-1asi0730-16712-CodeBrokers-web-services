using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;

/// <summary>
///     Converts validated creation input into an alert command.
/// </summary>
public static class CreateAlertCommandFromResourceAssembler
{
    public static CreateAlertCommand ToCommandFromResource(CreateAlertResource resource) => new(
        resource.OlderAdultId.GetValueOrDefault(),
        resource.HealthRecordId.GetValueOrDefault(),
        resource.MonitoringRuleId.GetValueOrDefault(),
        Enum.Parse<SeverityLevel>(resource.Severity!, ignoreCase: true));
}
