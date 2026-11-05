using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;

/// <summary>
///     Command to create a new health record.
/// </summary>
public record CreateHealthRecordCommand(
    Guid OlderAdultId,
    string RecordTypeCode,
    decimal? MeasurementValue,
    string? Note,
    RecordSource Source,
    Guid RecordedBy);