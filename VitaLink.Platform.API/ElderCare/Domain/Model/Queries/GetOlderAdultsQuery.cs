namespace VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

/// <summary>
///     Query to list and search older adults.
/// </summary>
/// <param name="Search">Text contained in the full name.</param>
/// <param name="CareProviderId">Only the older adults assigned to this care provider.</param>
/// <param name="IsMonitoringActive">Only those with the monitoring active or inactive.</param>
public sealed record GetOlderAdultsQuery(
    string? Search,
    Guid? CareProviderId,
    bool? IsMonitoringActive);
