namespace VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

/// <summary>
///     Query to get one older adult.
/// </summary>
/// <param name="OlderAdultId">Identifier of the older adult.</param>
public sealed record GetOlderAdultByIdQuery(Guid OlderAdultId);
