namespace VitaLink.Platform.API.Alerting.Domain.Model.Commands;

/// <summary>Requests an observation for an attended alert.</summary>
public sealed record CreateAlertObservationCommand(Guid AlertId, string Text, Guid CreatedBy);
