namespace VitaLink.ElderCare.Domain.Model.Commands;

/// <summary>
///     Command to associate an older adult with a care provider.
/// </summary>
/// <param name="OlderAdultId">
///     Identifier of the older adult.
/// </param>
/// <param name="CareProviderId">
///     Identifier of the care provider.
/// </param>
public sealed record LinkCareProviderCommand(
    Guid OlderAdultId,
    Guid CareProviderId);