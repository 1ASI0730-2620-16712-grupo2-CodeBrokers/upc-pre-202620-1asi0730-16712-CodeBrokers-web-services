using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.Alerting.Domain.Services;

/// <summary>Saves an observation for an attended alert.</summary>
public interface IAlertObservationCommandService
{
    Task<Result<AlertObservation, CreateAlertObservationError>> Handle(CreateAlertObservationCommand command,
        CancellationToken cancellationToken = default);
}
