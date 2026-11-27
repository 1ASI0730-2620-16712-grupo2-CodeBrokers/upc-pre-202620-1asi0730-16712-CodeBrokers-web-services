namespace VitaLink.Platform.API.IAM.Domain.Model.Commands;

/// <summary>
///     Command to register a family caregiver together with the patient and the first measurements.
/// </summary>
/// <param name="Email">Email the caregiver will sign in with.</param>
/// <param name="Password">Plain-text password typed by the caregiver.</param>
/// <param name="FirstName">Caregiver first name.</param>
/// <param name="LastName">Caregiver last name.</param>
/// <param name="Patient">Data of the patient.</param>
/// <param name="CareProviderId">Responsible care provider of the patient.</param>
/// <param name="Measurements">The first measurements of the patient.</param>
/// <param name="AuthorizationAccepted">Whether the caregiver confirmed they are authorized to register the patient.</param>
public sealed record RegisterFamilyCaregiverCommand(
    string? Email,
    string? Password,
    string? FirstName,
    string? LastName,
    RegistrationPatient Patient,
    Guid? CareProviderId,
    RegistrationMeasurements Measurements,
    bool AuthorizationAccepted);

/// <summary>
///     Patient data typed during the registration.
/// </summary>
public sealed record RegistrationPatient(
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Street,
    string? District,
    string? City);

/// <summary>
///     First measurements typed during the registration.
/// </summary>
public sealed record RegistrationMeasurements(
    decimal? HeartRate,
    decimal? OxygenSaturation,
    decimal? Glucose,
    decimal? SystolicPressure,
    decimal? DiastolicPressure);
