using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.Alerting.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="Alert" /> aggregate.
/// </summary>
public interface IAlertRepository : IBaseRepository<Alert, AlertId>
{
  /// <summary>
  ///     Counts the pending alerts assigned to a care provider.
  /// </summary>
  Task<int> CountPendingByCareProviderAsync(Guid careProviderId, CancellationToken cancellationToken = default);

  /// <summary>
  ///     Lists the alerts of a care provider ordered by severity, optionally filtered by status.
  /// </summary>
  Task<IEnumerable<Alert>> FindByCareProviderOrderBySeverityAsync(
    Guid careProviderId,
    AlertStatus? status,
    CancellationToken cancellationToken = default);

  /// <summary>
  ///     Lists the alerts of an older adult.
  /// </summary>
  Task<IEnumerable<Alert>> FindByOlderAdultAsync(Guid olderAdultId, CancellationToken cancellationToken = default);

  /// <summary>
  ///     Finds the alert raised by a health record, used to keep alert creation idempotent.
  /// </summary>
  Task<Alert?> FindByHealthRecordAsync(Guid healthRecordId, CancellationToken cancellationToken = default);
}
