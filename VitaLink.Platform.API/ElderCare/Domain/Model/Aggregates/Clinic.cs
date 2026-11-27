using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

/// <summary>
///     Clinic where care providers work. Providers without a clinic are independent.
/// </summary>
public class Clinic
{
    public ClinicId Id { get; private set; }
    public string Name { get; private set; }
    public string District { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Constructor utilizado por Entity Framework Core
    private Clinic()
    {
        Id = null!;
        Name = null!;
        District = null!;
    }

    public Clinic(string name, string district)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Clinic name cannot be empty.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(district))
            throw new ArgumentException(
                "Clinic district cannot be empty.",
                nameof(district));

        name = name.Trim();
        district = district.Trim();

        if (name.Length > 160)
            throw new ArgumentException(
                "Clinic name cannot exceed 160 characters.",
                nameof(name));

        if (district.Length > 80)
            throw new ArgumentException(
                "Clinic district cannot exceed 80 characters.",
                nameof(district));

        Id = ClinicId.New();
        Name = name;
        District = district;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
