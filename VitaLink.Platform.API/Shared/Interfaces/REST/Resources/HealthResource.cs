namespace VitaLink.Platform.API.Shared.Interfaces.REST.Resources;

/// <summary>
///     Resource returned by the health check endpoint.
/// </summary>
/// <param name="Status">The overall status of the API: <c>Healthy</c> or <c>Unhealthy</c>.</param>
/// <param name="Database">The status of the database connection: <c>Up</c> or <c>Down</c>.</param>
/// <param name="Timestamp">The UTC date and time of the check.</param>
public record HealthResource(string Status, string Database, DateTime Timestamp);
