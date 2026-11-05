using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a monitoring rule response.
/// </summary>
public record MonitoringRuleResource(
    Guid Id,
    Guid OlderAdultId,
    string RecordTypeCode,
    decimal? MinThreshold,
    decimal? MaxThreshold,
    string SeverityLevel,
    string Description,
    bool IsActive,
    DateTimeOffset CreatedAt);
    