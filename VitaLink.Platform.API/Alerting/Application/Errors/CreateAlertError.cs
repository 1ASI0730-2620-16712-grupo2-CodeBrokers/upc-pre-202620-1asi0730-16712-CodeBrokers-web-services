namespace VitaLink.Platform.API.Alerting.Application.Errors;

/// <summary>
///     Expected business failures when creating an alert.
/// </summary>
public enum CreateAlertError
{
    /// <summary>
    ///     The health record identifier is empty.
    /// </summary>
    InvalidReference,

    /// <summary>
    ///     The requested health record does not exist.
    /// </summary>
    HealthRecordNotFound
}
