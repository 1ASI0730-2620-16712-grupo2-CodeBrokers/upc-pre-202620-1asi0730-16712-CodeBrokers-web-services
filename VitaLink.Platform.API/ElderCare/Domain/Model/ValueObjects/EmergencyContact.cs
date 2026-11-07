using System.Text.RegularExpressions;

namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

public record EmergencyContact
{
    public string Name { get; init; }
    public string PhoneNumber { get; init; }

    public EmergencyContact(
        string name,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Emergency contact name cannot be empty.",
                nameof(name));

        name = name.Trim();

        if (name.Length > 160)
            throw new ArgumentException(
                "Emergency contact name cannot exceed 160 characters.",
                nameof(name));

        EnsureValidPhoneNumber(phoneNumber);

        Name = name;
        PhoneNumber = phoneNumber.Trim();
    }

    private static void EnsureValidPhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Phone number cannot be empty.",
                nameof(value));

        value = value.Trim();

        if (value.Length > 20)
            throw new ArgumentException(
                "Phone number cannot exceed 20 characters.",
                nameof(value));

        if (!Regex.IsMatch(value, @"^\+?[0-9]{7,15}$"))
            throw new ArgumentException(
                "Phone number has an invalid format.",
                nameof(value));
    }
}