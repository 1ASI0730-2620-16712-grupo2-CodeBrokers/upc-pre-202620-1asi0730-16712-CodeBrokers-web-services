using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;

public record CreateHealthRecordCommand(
    Guid OlderAdultId,
    string RecordTypeCode,
    decimal? MeasurementValue,
    string? Unit,
    string? Note,
    RecordSource Source,
    Guid RecordedBy);