namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;

/// <summary>
///     Expected business errors when creating a monitoring rule.
/// </summary>
public enum CreateMonitoringRuleError
{
    /// <summary>
    ///     The older adult identifier is empty.
    /// </summary>
    InvalidReference,

    /// <summary>
    ///     The record type is empty or does not exist.
    /// </summary>
    InvalidRecordType,

    /// <summary>
    ///     No limit is defined, or the minimum is greater than the maximum.
    /// </summary>
    InvalidThreshold,

    /// <summary>
    ///     An active rule already exists for the older adult, record type and severity.
    /// </summary>
    DuplicateActiveRule
}