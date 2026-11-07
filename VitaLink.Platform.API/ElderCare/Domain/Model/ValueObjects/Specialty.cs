namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

public record Specialty
{
    public string Value { get; init; }

    public Specialty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Specialty cannot be empty.",
                nameof(value));

        value = value.Trim();

        if (value.Length > 120)
            throw new ArgumentException(
                "Specialty cannot exceed 120 characters.",
                nameof(value));

        Value = value;
    }
}