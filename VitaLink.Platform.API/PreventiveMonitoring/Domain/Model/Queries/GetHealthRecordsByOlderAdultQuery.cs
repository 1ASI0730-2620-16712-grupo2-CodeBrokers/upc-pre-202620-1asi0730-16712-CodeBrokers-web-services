using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Queries;

public record GetHealthRecordsByOlderAdultQuery(
    Guid OlderAdultId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? RecordTypeCode);