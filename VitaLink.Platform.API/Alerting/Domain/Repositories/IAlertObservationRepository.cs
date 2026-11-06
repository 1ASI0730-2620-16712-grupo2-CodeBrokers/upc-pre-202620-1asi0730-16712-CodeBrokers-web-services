using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.Alerting.Domain.Repositories;

/// <summary>Persists and retrieves observations by alert.</summary>
public interface IAlertObservationRepository : IBaseRepository<AlertObservation, AlertObservationId>
{
    Task<AlertObservation?> FindByAlertAsync(Guid alertId, CancellationToken cancellationToken = default);
}
