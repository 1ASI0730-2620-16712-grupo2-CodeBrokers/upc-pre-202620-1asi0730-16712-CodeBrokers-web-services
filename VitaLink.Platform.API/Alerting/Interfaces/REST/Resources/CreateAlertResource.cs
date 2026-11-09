using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Identifies the health record to evaluate for alert generation.
/// </summary>
public record CreateAlertResource(
    [Required(ErrorMessage = "AlertingHealthRecordIdRequired")] Guid? HealthRecordId);
