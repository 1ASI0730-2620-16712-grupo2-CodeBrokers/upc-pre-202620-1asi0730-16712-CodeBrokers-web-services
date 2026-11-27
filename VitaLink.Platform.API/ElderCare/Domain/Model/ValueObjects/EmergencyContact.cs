using System.Text.RegularExpressions;

namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

/// <summary>
///     Emergency contact of an older adult.
/// </summary>
/// <remarks>
///     Both parts are optional so that an incomplete profile can be saved while the monitoring is inactive;
///     the application requires both of them before the monitoring can be activated.
/// </remarks>
public record EmergencyContact
{
    public string? Name { get; init; }
    public string? PhoneNumber { get; init; }

    public EmergencyContact(
        string? name,
        string? phoneNumber)
    {
        name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        phoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();

        if (name is null && phoneNumber is null)
            throw new ArgumentException(
                "An emergency contact needs a name or a phone number.",
                nameof(name));

        if (name?.Length > 160)
            throw new ArgumentException(
                "Emergency contact name cannot exceed 160 characters.",
                nameof(name));

        if (phoneNumber is not null)
            EnsureValidPhoneNumber(phoneNumber);

        Name = name;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    ///     Indicates whether a phone number has 7 to 20 characters, only digits and the symbols
    ///     <c>+ ( ) -</c> or spaces, and at least one digit.
    /// </summary>
    public static bool IsValidPhoneNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        value = value.Trim();

        return value.Length is >= 7 and <= 20
               && Regex.IsMatch(value, @"^[0-9+() \-]+$")
               && Regex.IsMatch(value, @"[0-9]");
    }

    private static void EnsureValidPhoneNumber(string value)
    {
        if (!IsValidPhoneNumber(value))
            throw new ArgumentException(
                "Phone number must have 7 to 20 characters, with digits and the symbols + ( ) - or spaces.",
                nameof(value));
    }
}
