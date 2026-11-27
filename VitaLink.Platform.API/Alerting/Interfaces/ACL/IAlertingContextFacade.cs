namespace VitaLink.Platform.API.Alerting.Interfaces.ACL;

/// <summary>
///     Lets other bounded contexts ask Alerting to evaluate saved health records.
/// </summary>
public interface IAlertingContextFacade
{
    /// <summary>
    ///     Evaluates each health record; the alerts that are raised send their email notification.
    ///     A failure never propagates: the records are already saved.
    /// </summary>
    /// <returns>The number of alerts created.</returns>
    Task<int> GenerateAlertsAsync(
        IEnumerable<Guid> healthRecordIds,
        CancellationToken cancellationToken = default);
}
