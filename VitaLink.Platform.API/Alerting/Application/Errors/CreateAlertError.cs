namespace VitaLink.Platform.API.Alerting.Application.Errors;

/// <summary>
///     Expected business failures when creating an alert.
/// </summary>
public enum CreateAlertError
{
    /// <summary>
    ///     An alert already exists for the health record.
    /// </summary>
    AlertAlreadyRaised,

    /// <summary>
    ///     A required identifier is empty or the health record belongs to another older adult.
    /// </summary>
    InvalidReference,

    /// <summary>
    ///     The requested health record does not exist.
    /// </summary>
    HealthRecordNotFound,

    /// <summary>
    ///     The health record does not contain a measurement that can be evaluated against thresholds.
    /// </summary>
    UnsupportedHealthRecord
}
