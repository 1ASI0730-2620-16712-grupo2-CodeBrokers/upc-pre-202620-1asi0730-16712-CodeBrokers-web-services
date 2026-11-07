namespace VitaLink.Platform.API.Alerting.Domain.Model.Commands;

/// <summary>
///     Requests evaluation of a health record for alert generation.
/// </summary>
public record GenerateAlertFromHealthRecordCommand(Guid HealthRecordId);
