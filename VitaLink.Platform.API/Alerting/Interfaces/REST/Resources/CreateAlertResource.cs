using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Input for creating an alert from explicit references and a severity level.
/// </summary>
public record CreateAlertResource(
    [Required(ErrorMessage = "AlertingInvalidReference")] Guid? OlderAdultId,
    [Required(ErrorMessage = "AlertingInvalidReference")] Guid? HealthRecordId,
    [Required(ErrorMessage = "AlertingInvalidReference")] Guid? MonitoringRuleId,
    [Required(ErrorMessage = "AlertingInvalidSeverity")]
    [RegularExpression("^(HIGH|MEDIUM|LOW)$", ErrorMessage = "AlertingInvalidSeverity")] string? Severity);
