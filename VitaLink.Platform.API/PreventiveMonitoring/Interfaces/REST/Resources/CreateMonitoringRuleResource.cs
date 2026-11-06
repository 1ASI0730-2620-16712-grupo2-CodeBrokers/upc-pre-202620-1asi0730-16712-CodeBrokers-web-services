using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

/// <summary>
///     Resource representing the payload to create a monitoring rule.
/// </summary>
public record CreateMonitoringRuleResource(
    Guid OlderAdultId,
    string RecordTypeCode,
    decimal? MinThreshold,
    decimal? MaxThreshold,
    string Severity);