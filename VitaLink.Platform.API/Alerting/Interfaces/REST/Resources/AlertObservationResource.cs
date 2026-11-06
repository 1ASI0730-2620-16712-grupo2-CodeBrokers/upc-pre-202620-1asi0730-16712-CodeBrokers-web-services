namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>Returns the saved observation and its attribution.</summary>
public record AlertObservationResource(Guid Id, Guid AlertId, string Text, Guid CreatedBy, DateTimeOffset CreatedAt);
