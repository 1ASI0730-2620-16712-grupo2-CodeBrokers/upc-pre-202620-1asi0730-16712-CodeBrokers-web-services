namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Queries;

/// <summary>
///     Query to get the most recent measurement of each type for an older adult.
/// </summary>
/// <param name="OlderAdultId">Identifier of the older adult.</param>
public sealed record GetLatestHealthRecordsByOlderAdultQuery(Guid OlderAdultId);
