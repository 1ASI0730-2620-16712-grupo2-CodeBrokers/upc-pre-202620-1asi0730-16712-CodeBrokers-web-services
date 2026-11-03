using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;

/// <summary>
///     Maps alerts to resources without exposing domain objects or navigation properties.
/// </summary>
public static class AlertResourceFromEntityAssembler
{
    public static AlertResource ToResourceFromEntity(Alert alert) => new(
        alert.Id.Value, alert.OlderAdultId, alert.HealthRecordId, alert.MonitoringRuleId,
        alert.AssignedProviderId, alert.Severity.ToString().ToUpperInvariant(),
        alert.Status == AlertStatus.InReview ? "IN_REVIEW" : alert.Status.ToString().ToUpperInvariant(),
        alert.RaisedAt, alert.LastStatusChangeAt);
}
