namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

/// <summary>
///     Lets other bounded contexts save the first measurements of a patient and their monitoring rules.
/// </summary>
public interface IMonitoringRegistrationFacade
{
    /// <summary>
    ///     Checks the measurements against the valid ranges of the record types without saving anything.
    /// </summary>
    Task<IReadOnlyList<MeasurementIssue>> ValidateAsync(
        InitialMeasurements measurements,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves one health record per measurement and the default monitoring rules of the patient.
    /// </summary>
    /// <returns>The identifiers of the saved health records.</returns>
    Task<IReadOnlyList<Guid>> SaveAsync(
        Guid olderAdultId,
        Guid recordedBy,
        InitialMeasurements measurements,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     The five values typed when the family caregiver registers a patient.
/// </summary>
public record InitialMeasurements(
    decimal? HeartRate,
    decimal? OxygenSaturation,
    decimal? Glucose,
    decimal? SystolicPressure,
    decimal? DiastolicPressure);

/// <summary>
///     A measurement that is not valid.
/// </summary>
public record MeasurementIssue(
    string Field,
    MeasurementIssueKind Kind,
    decimal? Min = null,
    decimal? Max = null);

public enum MeasurementIssueKind
{
    Required,
    OutOfRange,
    PressureOrder
}
