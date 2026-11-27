namespace VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Body of the family caregiver registration: account, patient and first measurements.
/// </summary>
/// <remarks>
///     The fields are validated by the registration rules (not by annotations), so every problem is reported
///     together and nothing is saved when any of them fails.
/// </remarks>
/// <param name="Email">Email the caregiver will sign in with.</param>
/// <param name="Password">Plain-text password.</param>
/// <param name="FirstName">Caregiver first name.</param>
/// <param name="LastName">Caregiver last name.</param>
/// <param name="Patient">Data of the patient.</param>
/// <param name="CareProviderId">Responsible care provider of the patient.</param>
/// <param name="Measurements">The first measurements of the patient.</param>
/// <param name="AuthorizationAccepted">The caregiver confirms they are authorized to register the patient.</param>
public sealed record RegisterFamilyCaregiverResource(
    string? Email,
    string? Password,
    string? FirstName,
    string? LastName,
    RegistrationPatientResource? Patient,
    Guid? CareProviderId,
    RegistrationMeasurementsResource? Measurements,
    bool AuthorizationAccepted);

/// <summary>
///     Patient data of the registration.
/// </summary>
/// <param name="FirstName">Patient first name.</param>
/// <param name="LastName">Patient last name.</param>
/// <param name="BirthDate">Birth date (yyyy-MM-dd), not in the future.</param>
/// <param name="Street">Street of the address (optional).</param>
/// <param name="District">District of the address (optional).</param>
/// <param name="City">City of the address (optional).</param>
public sealed record RegistrationPatientResource(
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Street,
    string? District,
    string? City);

/// <summary>
///     First measurements of the registration.
/// </summary>
/// <param name="HeartRate">Pulse in bpm.</param>
/// <param name="OxygenSaturation">Oxygen saturation in %.</param>
/// <param name="Glucose">Blood glucose in mg/dL.</param>
/// <param name="SystolicPressure">Systolic blood pressure in mmHg.</param>
/// <param name="DiastolicPressure">Diastolic blood pressure in mmHg.</param>
public sealed record RegistrationMeasurementsResource(
    decimal? HeartRate,
    decimal? OxygenSaturation,
    decimal? Glucose,
    decimal? SystolicPressure,
    decimal? DiastolicPressure);
