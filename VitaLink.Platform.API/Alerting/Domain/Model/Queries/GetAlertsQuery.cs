using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Queries;

/// <summary>
///     Filters and pagination for alert searches.
/// </summary>
public record GetAlertsQuery(AlertStatus? Status, Guid? OlderAdultId, int Page = 1, int PageSize = 20);
