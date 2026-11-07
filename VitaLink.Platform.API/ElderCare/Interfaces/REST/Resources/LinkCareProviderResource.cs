using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.ElderCare.Interfaces.REST.Resources;

/// <summary>
///     Request body for associating a care provider with an older adult.
/// </summary>
/// <param name="CareProviderId">
///     Identifier of the care provider to associate.
/// </param>
public sealed record LinkCareProviderResource(
    [Required] Guid CareProviderId);