using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Domain.Services;

namespace VitaLink.Platform.API.Alerting.Application.Internal.QueryServices;

/// <summary>Reads the saved observation without tracking changes.</summary>
public class AlertObservationQueryService(IAlertObservationRepository repository) : IAlertObservationQueryService
{
    public Task<AlertObservation?> Handle(Guid alertId, CancellationToken cancellationToken = default) =>
        repository.FindByAlertAsync(alertId, cancellationToken);
}
