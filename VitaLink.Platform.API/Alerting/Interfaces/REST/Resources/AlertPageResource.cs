namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Paginated alerts with totals and the requested page coordinates.
/// </summary>
public record AlertPageResource(IReadOnlyList<AlertResource> Items, int TotalItems, int Page, int PageSize,
    int TotalPages);
