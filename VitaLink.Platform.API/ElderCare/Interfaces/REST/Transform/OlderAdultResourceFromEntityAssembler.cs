using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

/// <summary>
///     Converts an older adult into its REST resource.
/// </summary>
public static class OlderAdultResourceFromEntityAssembler
{
    public static OlderAdultResource ToResourceFromEntity(OlderAdult olderAdult)
    {
        return new OlderAdultResource(
            olderAdult.Id.Value,
            olderAdult.FullName.FirstName,
            olderAdult.FullName.LastName,
            olderAdult.BirthDate,
            olderAdult.Age(),
            olderAdult.Address?.Street,
            olderAdult.Address?.District,
            olderAdult.Address?.City,
            olderAdult.EmergencyContact?.Name,
            olderAdult.EmergencyContact?.PhoneNumber,
            olderAdult.AssignedProviderId,
            olderAdult.IsMonitoringActive,
            olderAdult.CreatedAt);
    }
}
