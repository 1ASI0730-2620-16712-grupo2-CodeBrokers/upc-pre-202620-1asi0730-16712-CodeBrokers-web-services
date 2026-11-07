namespace VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

public record Address
{
    public string? Street { get; init; }
    public string? District { get; init; }
    public string? City { get; init; }

    public Address(
        string? street,
        string? district,
        string? city)
    {
        street = street?.Trim();
        district = district?.Trim();
        city = city?.Trim();

        if (street?.Length > 160)
            throw new ArgumentException(
                "Street cannot exceed 160 characters.",
                nameof(street));

        if (district?.Length > 80)
            throw new ArgumentException(
                "District cannot exceed 80 characters.",
                nameof(district));

        if (city?.Length > 80)
            throw new ArgumentException(
                "City cannot exceed 80 characters.",
                nameof(city));

        Street = street;
        District = district;
        City = city;
    }
}