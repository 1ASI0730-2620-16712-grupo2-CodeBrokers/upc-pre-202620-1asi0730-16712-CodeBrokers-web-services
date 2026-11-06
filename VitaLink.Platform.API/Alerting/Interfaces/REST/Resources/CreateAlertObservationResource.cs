namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>Accepts an optional observation and its demonstration author.</summary>
public record CreateAlertObservationResource(string? Text, Guid? CreatedBy);
