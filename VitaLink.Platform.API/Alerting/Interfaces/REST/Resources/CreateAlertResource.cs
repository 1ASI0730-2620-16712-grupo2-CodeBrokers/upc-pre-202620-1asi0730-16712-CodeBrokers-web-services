using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Identifies the health record to evaluate for alert generation.
/// </summary>
/// <remarks>
///     The priority (severity) is derived by the server from the monitoring rules (US-15, CA-03):
///     a request that sends <c>severity</c> is rejected with HTTP 400.
/// </remarks>
/// <param name="HealthRecordId">Health record to evaluate.</param>
/// <param name="Severity">Not allowed. Present only to reject requests that try to impose the priority.</param>
public record CreateAlertResource(
    [Required(ErrorMessage = "AlertingHealthRecordIdRequired")] Guid? HealthRecordId,
    [RegularExpression("^$", ErrorMessage = "AlertingSeverityNotAllowed")] string? Severity = null);
