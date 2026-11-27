namespace VitaLink.Platform.API.IAM.Domain.Model.Errors;

/// <summary>
///     Reasons why the registration of a family caregiver can fail.
/// </summary>
public abstract record RegisterFamilyCaregiverError
{
    /// <summary>One or more fields are missing or not valid. Nothing was saved.</summary>
    public sealed record InvalidFields(IReadOnlyList<RegistrationFieldError> Fields) : RegisterFamilyCaregiverError;

    /// <summary>Another account already uses the email.</summary>
    public sealed record EmailAlreadyRegistered : RegisterFamilyCaregiverError;

    /// <summary>Unexpected failure.</summary>
    public sealed record Unexpected : RegisterFamilyCaregiverError;
}

/// <summary>
///     A field of the registration that is not valid.
/// </summary>
/// <param name="Field">Name of the field as it appears in the request body (for example <c>patient.birthDate</c>).</param>
/// <param name="Code">Why the field is not valid.</param>
/// <param name="Limit">Maximum length, for <see cref="RegistrationFieldCode.TooLong"/>.</param>
/// <param name="Min">Lower valid value, for <see cref="RegistrationFieldCode.OutOfRange"/>.</param>
/// <param name="Max">Upper valid value, for <see cref="RegistrationFieldCode.OutOfRange"/>.</param>
public sealed record RegistrationFieldError(
    string Field,
    RegistrationFieldCode Code,
    int? Limit = null,
    decimal? Min = null,
    decimal? Max = null);

/// <summary>
///     Why a field of the registration is not valid.
/// </summary>
public enum RegistrationFieldCode
{
    Required,
    TooLong,
    InvalidEmail,
    InvalidPassword,
    InvalidBirthDate,
    OutOfRange,
    PressureOrder,
    AuthorizationRequired,
    CareProviderNotFound,
    CareProviderInactive
}
