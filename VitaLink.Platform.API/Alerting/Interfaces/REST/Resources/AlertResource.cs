namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Public representation of an alert with textual state and severity.
/// </summary>
public record AlertResource(Guid Id, Guid OlderAdultId, Guid HealthRecordId, Guid MonitoringRuleId,
    Guid? AssignedProviderId, string Severity, string Status, DateTimeOffset RaisedAt,
    DateTimeOffset LastStatusChangeAt);
