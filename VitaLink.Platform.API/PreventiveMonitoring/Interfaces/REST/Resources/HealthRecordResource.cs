using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

public record HealthRecordResource(
    Guid Id,
    Guid OlderAdultId,
    string RecordTypeCode,
    decimal? MeasurementValue,
    string? Note,
    string Source,
    Guid RecordedBy,
    DateTimeOffset RecordedAt);