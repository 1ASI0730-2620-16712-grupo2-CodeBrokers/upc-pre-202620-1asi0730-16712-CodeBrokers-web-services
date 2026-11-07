namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Response returned after an alert status update.
/// </summary>
/// <param name="Id">Identifier of the alert.</param>
/// <param name="Status">Current alert status.</param>
/// <param name="LastStatusChangeAt">Timestamp of the latest status change.</param>
/// <param name="Version">Current PostgreSQL row version.</param>
public sealed record AlertStatusResource(
    Guid Id,
    string Status,
    DateTimeOffset LastStatusChangeAt,
    uint Version);