namespace VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>
///     Lifecycle status of an alert.
/// </summary>
public enum AlertStatus
{
  Pending,
  InReview,
  Attended,
  Closed
}
