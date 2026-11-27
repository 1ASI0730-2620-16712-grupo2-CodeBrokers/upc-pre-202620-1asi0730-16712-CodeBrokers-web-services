namespace VitaLink.Platform.API.Alerting.Domain.Model.Queries;

/// <summary>
///     Query to read the status history of several alerts in a single call.
/// </summary>
/// <param name="AlertIds">Identifiers of the alerts (1 to <see cref="MaxAlerts" />).</param>
public sealed record GetAlertsStatusHistoryQuery(IReadOnlyCollection<Guid> AlertIds)
{
    /// <summary>
    ///     Maximum number of alerts allowed in one request.
    /// </summary>
    public const int MaxAlerts = 100;
}
