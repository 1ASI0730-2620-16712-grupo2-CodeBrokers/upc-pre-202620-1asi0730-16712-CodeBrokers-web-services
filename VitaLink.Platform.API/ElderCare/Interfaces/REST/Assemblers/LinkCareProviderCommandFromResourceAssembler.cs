using VitaLink.ElderCare.Domain.Model.Commands;
using VitaLink.ElderCare.Interfaces.REST.Resources;

namespace VitaLink.ElderCare.Interfaces.REST.Assemblers;

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