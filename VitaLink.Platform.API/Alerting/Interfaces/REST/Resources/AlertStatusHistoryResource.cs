namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

/// <summary>
///     Status history of one alert, returned by the batch history endpoint.
/// </summary>
/// <param name="AlertId">The alert.</param>
/// <param name="History">Status changes, oldest first.</param>
public sealed record AlertStatusHistoryResource(Guid AlertId, IEnumerable<AlertStatusChangeResource> History);
