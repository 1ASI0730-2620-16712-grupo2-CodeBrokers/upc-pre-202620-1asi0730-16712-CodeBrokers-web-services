using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Entities;

/// <summary>
///     Records one status transition of an alert, keeping its full history (US-19).
/// </summary>
public class AlertStatusChange
{
  protected AlertStatusChange()
  {
  }

  public AlertStatusChange(AlertStatus fromStatus, AlertStatus toStatus, Guid changedBy)
  {
    Id = AlertStatusChangeId.New();
    FromStatus = fromStatus;
    ToStatus = toStatus;
    ChangedBy = changedBy;
    ChangedAt = DateTimeOffset.UtcNow;
  }

  public AlertStatusChangeId Id { get; private set; } = null!;

  public AlertStatus FromStatus { get; private set; }

  public AlertStatus ToStatus { get; private set; }

  public Guid ChangedBy { get; private set; }

  public DateTimeOffset ChangedAt { get; private set; }

  /// <summary>
  ///     Returns a readable description of the transition.
  /// </summary>
  public string Describe() => $"{FromStatus} -> {ToStatus}";
}
