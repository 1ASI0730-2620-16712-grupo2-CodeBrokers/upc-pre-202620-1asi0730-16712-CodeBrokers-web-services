namespace VitaLink.Platform.API.Alerting.Application.Errors;

/// <summary>Expected failures when saving an observation.</summary>
public enum CreateAlertObservationError
{
    AlertNotFound,
    AlertNotAttended,
    ObservationAlreadyExists,
    InvalidReference,
    TextTooLong
}
