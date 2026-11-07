namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Identifies the evaluated record and its alert, when one exists.
/// </summary>
public record AlertEvaluationResource(Guid HealthRecordId, AlertResource? Alert);
