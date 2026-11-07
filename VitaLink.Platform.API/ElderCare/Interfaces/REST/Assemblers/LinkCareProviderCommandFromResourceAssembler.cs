using VitaLink.Platform.API.ElderCare.Domain.Model.Commands;
using VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Assemblers;

/// <summary>
///     Converts a REST resource into a command for linking a care provider.
/// </summary>
public static class LinkCareProviderCommandFromResourceAssembler
{
    public static LinkCareProviderCommand ToCommandFromResource(
        Guid olderAdultId,
        LinkCareProviderResource resource)
    {
        return new LinkCareProviderCommand(
            olderAdultId,
            resource.CareProviderId);
    }
}