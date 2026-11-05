using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Commands;

/// <summary>
///     Command to create a new alert from a health record that broke a monitoring rule.
/// </summary>
/// <param name="OlderAdultId">Older adult the alert is about.</param>
/// <param name="HealthRecordId">Health record that triggered the alert.</param>
/// <param name="MonitoringRuleId">Monitoring rule that was broken.</param>
/// <param name="Severity">Severity level of the alert.</param>
public record CreateAlertCommand(
  Guid OlderAdultId,
  Guid HealthRecordId,
  Guid MonitoringRuleId,
  SeverityLevel Severity);
