namespace VitaLink.Platform.API.Shared.Interfaces.REST.Resources;

/// <summary>
///     Resource returned by the health check endpoint.
/// </summary>
/// <param name="Status">The current status of the API.</param>
/// <param name="Timestamp">The UTC date and time of the check.</param>
public record HealthResource(string Status, DateTime Timestamp);
