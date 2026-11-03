using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Services;

/// <summary>
///     Thrown when an alert is asked to move to a status that its state machine does not allow.
/// </summary>
public class InvalidAlertTransitionException : Exception
{
  public InvalidAlertTransitionException(AlertStatus from, AlertStatus to)
    : base($"Invalid alert status transition from {from} to {to}.")
  {
    From = from;
    To = to;
  }

  public AlertStatus From { get; }

  public AlertStatus To { get; }
}
