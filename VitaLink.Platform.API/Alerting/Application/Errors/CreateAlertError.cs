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
    ///     An older adult, health record or monitoring rule identifier is empty.
    /// </summary>
    InvalidReference
}
