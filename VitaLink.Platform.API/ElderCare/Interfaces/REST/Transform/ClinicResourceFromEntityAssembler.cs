using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

/// <summary>
///     Converts a clinic into its REST resource.
/// </summary>
public static class ClinicResourceFromEntityAssembler
{
    public static ClinicResource ToResourceFromEntity(Clinic entity)
    {
        return new ClinicResource(
            entity.Id.Value,
            entity.Name,
            entity.District);
    }
}
