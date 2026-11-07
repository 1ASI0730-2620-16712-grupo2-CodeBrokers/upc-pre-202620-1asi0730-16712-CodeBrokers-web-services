using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;

/// <summary>Converts a saved observation into its REST representation.</summary>
public static class AlertObservationResourceFromEntityAssembler
{
    public static AlertObservationResource ToResourceFromEntity(AlertObservation observation) =>
        new(observation.Id.Value, observation.AlertId.Value, observation.Text, observation.CreatedBy, observation.CreatedAt);
}
