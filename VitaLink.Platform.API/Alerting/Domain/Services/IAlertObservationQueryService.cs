using VitaLink.Platform.API.Alerting.Domain.Model.Entities;

namespace VitaLink.Platform.API.Alerting.Domain.Services;

/// <summary>Retrieves the observation for an alert.</summary>
public interface IAlertObservationQueryService
{
    Task<AlertObservation?> Handle(Guid alertId, CancellationToken cancellationToken = default);
}
