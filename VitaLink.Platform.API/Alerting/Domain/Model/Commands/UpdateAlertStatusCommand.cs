namespace VitaLink.Platform.API.Alerting.Domain.Model.Commands;

/// <summary>
///     Command to change an alert status with optimistic concurrency.
/// </summary>
/// <param name="AlertId">Identifier of the alert.</param>
/// <param name="Status">Requested status.</param>
/// <param name="ChangedBy">User performing the change.</param>
/// <param name="Version">Expected PostgreSQL row version.</param>
public sealed record UpdateAlertStatusCommand(
    Guid AlertId,
    string Status,
    Guid ChangedBy,
    uint Version);