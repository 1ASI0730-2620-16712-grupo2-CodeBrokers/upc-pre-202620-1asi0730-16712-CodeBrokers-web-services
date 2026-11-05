using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Request body for updating an alert status.
/// </summary>
public sealed record UpdateAlertStatusResource(
    [Required]
    string Status,

    [Required]
    Guid ChangedBy,

    [Required]
    uint Version);