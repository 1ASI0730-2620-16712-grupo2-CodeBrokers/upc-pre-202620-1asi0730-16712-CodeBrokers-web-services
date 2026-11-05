using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Request body for updating an alert status.
/// </summary>
public sealed record UpdateAlertStatusResource(
    [property: Required]
    string Status,

    [property: Required]
    Guid ChangedBy,

    [property: Required]
    uint Version);