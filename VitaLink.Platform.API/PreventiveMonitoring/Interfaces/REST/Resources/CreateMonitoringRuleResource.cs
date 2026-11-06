using System;
using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

/// <summary>
///     Resource representing the payload to create a monitoring rule.
/// </summary>
public record CreateMonitoringRuleResource(
    [Required(ErrorMessage = "MonitoringRuleInvalidReference")] Guid? OlderAdultId,
    [Required(ErrorMessage = "MonitoringRuleInvalidRecordType")] string? RecordTypeCode,
    decimal? MinThreshold,
    decimal? MaxThreshold,
    [Required(ErrorMessage = "MonitoringRuleInvalidSeverity")]
    [RegularExpression("^(HIGH|MEDIUM|LOW)$", ErrorMessage = "MonitoringRuleInvalidSeverity")] string? Severity);