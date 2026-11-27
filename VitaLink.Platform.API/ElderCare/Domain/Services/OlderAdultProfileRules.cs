using VitaLink.Platform.API.ElderCare.Domain.Model.Errors;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.ElderCare.Domain.Services;

/// <summary>
///     Validation rules of the basic profile of an older adult (US-20).
/// </summary>
/// <remarks>
///     Identification (first name, last name, birth date) is always required. The address and the emergency
///     contact are required only to activate the monitoring; an incomplete profile can be saved inactive.
///     No required text accepts only spaces.
/// </remarks>
public static class OlderAdultProfileRules
{
    public const int NameMaxLength = 100;
    public const int StreetMaxLength = 160;
    public const int DistrictMaxLength = 80;
    public const int CityMaxLength = 80;
    public const int ContactNameMaxLength = 160;

    /// <summary>
    ///     Returns the fields that are not valid; an empty list means the profile is valid.
    /// </summary>
    /// <param name="today">Current date in UTC, used to reject future birth dates.</param>
    public static IReadOnlyList<FieldError> Validate(
        string? firstName,
        string? lastName,
        DateOnly? birthDate,
        string? street,
        string? district,
        string? city,
        string? emergencyContactName,
        string? emergencyContactPhone,
        bool isMonitoringActive,
        DateOnly today)
    {
        var errors = new List<FieldError>();

        CheckText(errors, "firstName", firstName, NameMaxLength, FieldErrorCode.Required, true);
        CheckText(errors, "lastName", lastName, NameMaxLength, FieldErrorCode.Required, true);

        if (birthDate is null || birthDate == default(DateOnly) || birthDate > today)
            errors.Add(new FieldError("birthDate", FieldErrorCode.InvalidBirthDate));

        CheckText(errors, "street", street, StreetMaxLength, FieldErrorCode.RequiredToActivate, isMonitoringActive);
        CheckText(errors, "district", district, DistrictMaxLength, FieldErrorCode.RequiredToActivate, isMonitoringActive);
        CheckText(errors, "city", city, CityMaxLength, FieldErrorCode.RequiredToActivate, isMonitoringActive);
        CheckText(
            errors,
            "emergencyContactName",
            emergencyContactName,
            ContactNameMaxLength,
            FieldErrorCode.RequiredToActivate,
            isMonitoringActive);

        if (string.IsNullOrWhiteSpace(emergencyContactPhone))
        {
            if (isMonitoringActive)
                errors.Add(new FieldError("emergencyContactPhone", FieldErrorCode.RequiredToActivate));
        }
        else if (!EmergencyContact.IsValidPhoneNumber(emergencyContactPhone))
        {
            errors.Add(new FieldError("emergencyContactPhone", FieldErrorCode.InvalidPhone));
        }

        return errors;
    }

    private static void CheckText(
        List<FieldError> errors,
        string field,
        string? value,
        int maxLength,
        FieldErrorCode requiredCode,
        bool required)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
                errors.Add(new FieldError(field, requiredCode));

            return;
        }

        if (value.Trim().Length > maxLength)
            errors.Add(new FieldError(field, FieldErrorCode.TooLong, maxLength));
    }
}
