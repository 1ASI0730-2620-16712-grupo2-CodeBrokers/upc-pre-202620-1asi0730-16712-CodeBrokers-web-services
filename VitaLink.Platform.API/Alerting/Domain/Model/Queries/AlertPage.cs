using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Queries;

/// <summary>
///     A page of alerts and the total number of matching alerts.
/// </summary>
public record AlertPage(IReadOnlyList<Alert> Items, int TotalItems, int Page, int PageSize);
