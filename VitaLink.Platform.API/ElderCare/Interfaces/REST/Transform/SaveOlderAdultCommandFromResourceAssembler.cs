using VitaLink.Platform.API.ElderCare.Domain.Model.Commands;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Transform;

/// <summary>
///     Converts the save resource into the create and update commands.
/// </summary>
public static class SaveOlderAdultCommandFromResourceAssembler
{
    public static CreateOlderAdultCommand ToCreateCommand(SaveOlderAdultResource resource)
    {
        return new CreateOlderAdultCommand(
            resource.FirstName,
            resource.LastName,
            resource.BirthDate,
            resource.Street,
            resource.District,
            resource.City,
            resource.EmergencyContactName,
            resource.EmergencyContactPhone,
            resource.IsMonitoringActive,
            resource.CareProviderId);
    }

    public static UpdateOlderAdultCommand ToUpdateCommand(
        Guid olderAdultId,
        SaveOlderAdultResource resource)
    {
        return new UpdateOlderAdultCommand(
            olderAdultId,
            resource.FirstName,
            resource.LastName,
            resource.BirthDate,
            resource.Street,
            resource.District,
            resource.City,
            resource.EmergencyContactName,
            resource.EmergencyContactPhone,
            resource.IsMonitoringActive);
    }
}
