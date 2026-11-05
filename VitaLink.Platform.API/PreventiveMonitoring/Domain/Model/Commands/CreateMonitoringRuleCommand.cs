using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;

/// <summary>
///     Command to create a new monitoring rule.
/// </summary>
public record CreateMonitoringRuleCommand(
    Guid OlderAdultId,
    string RecordTypeCode,
    decimal? MinThreshold,
    decimal? MaxThreshold,
    string SeverityLevel,
    string Description);