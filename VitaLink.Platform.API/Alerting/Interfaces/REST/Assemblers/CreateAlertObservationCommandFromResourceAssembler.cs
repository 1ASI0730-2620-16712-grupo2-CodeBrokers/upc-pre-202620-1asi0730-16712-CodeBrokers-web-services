using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;

/// <summary>Converts the request into an observation command.</summary>
public static class CreateAlertObservationCommandFromResourceAssembler
{
    public static CreateAlertObservationCommand ToCommandFromResource(Guid alertId, CreateAlertObservationResource resource) =>
        new(alertId, resource.Text ?? string.Empty, resource.CreatedBy.GetValueOrDefault());
}
