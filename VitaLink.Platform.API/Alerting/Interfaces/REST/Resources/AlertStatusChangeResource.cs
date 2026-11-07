namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Represents one alert status transition in the REST API.
/// </summary>
public sealed record AlertStatusChangeResource(
    Guid Id,
    string FromStatus,
    string ToStatus,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);