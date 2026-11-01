using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root that represents an alert raised when a health record breaks a monitoring rule.
/// </summary>
public class Alert
{
  private readonly List<AlertStatusChange> _statusHistory = [];

  protected Alert()
  {
  }

  public Alert(CreateAlertCommand command)
  {
    ArgumentNullException.ThrowIfNull(command);

    Id = AlertId.New();
    OlderAdultId = command.OlderAdultId;
    HealthRecordId = command.HealthRecordId;
    MonitoringRuleId = command.MonitoringRuleId;
    Severity = command.Severity;
    Status = AlertStatus.Pending;
    RaisedAt = DateTimeOffset.UtcNow;
    LastStatusChangeAt = RaisedAt;
  }

  public AlertId Id { get; private set; } = null!;

  public Guid OlderAdultId { get; private set; }

  public Guid HealthRecordId { get; private set; }

  public Guid MonitoringRuleId { get; private set; }

  public Guid? AssignedProviderId { get; private set; }

  public SeverityLevel Severity { get; private set; }

  public AlertStatus Status { get; private set; }

  public DateTimeOffset RaisedAt { get; private set; }

  public DateTimeOffset LastStatusChangeAt { get; private set; }

  public IReadOnlyCollection<AlertStatusChange> StatusHistory => _statusHistory;

  /// <summary>
  ///     Indicates whether the alert is still waiting to be reviewed.
  /// </summary>
  public bool IsPending() => Status == AlertStatus.Pending;
}
