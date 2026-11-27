using VitaLink.Platform.API.Alerting.Interfaces.ACL;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Outbound service that asks the Alerting bounded context to evaluate a saved health record.
/// </summary>
/// <remarks>
///     PreventiveMonitoring only talks to Alerting through <see cref="IAlertingContextFacade" /> (anti-corruption layer).
/// </remarks>
/// <param name="alertingContextFacade">Facade exposed by the Alerting bounded context.</param>
public class ExternalAlertingService(IAlertingContextFacade alertingContextFacade)
{
    /// <summary>
    ///     Evaluates the monitoring rules for the health record and raises the alert when a rule is violated.
    /// </summary>
    /// <param name="healthRecordId">The saved health record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> when an alert was created.</returns>
    public async Task<bool> EvaluateHealthRecordAsync(Guid healthRecordId, CancellationToken cancellationToken = default)
    {
        var created = await alertingContextFacade.GenerateAlertsAsync([healthRecordId], cancellationToken);
        return created > 0;
    }
}
