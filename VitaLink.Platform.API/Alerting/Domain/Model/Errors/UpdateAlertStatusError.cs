namespace VitaLink.Platform.API.Alerting.Domain.Model.Errors;

/// <summary>
///     Expected business errors when updating an alert status.
/// </summary>
public enum UpdateAlertStatusError
{
    AlertNotFound,
    InvalidTransition,
    ConcurrencyConflict
}