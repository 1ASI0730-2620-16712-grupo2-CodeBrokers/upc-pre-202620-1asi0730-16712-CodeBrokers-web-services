using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

/// <summary>
///     Entry of the simulated registry of professional licenses.
/// </summary>
/// <remarks>
///     The project has no access to a real professional registry. This table, filled with test data,
///     stands in for it: a license is valid when it appears here.
/// </remarks>
public class RegisteredLicense
{
    public Guid Id { get; private set; }
    public string LicenseNumber { get; private set; }
    public string HolderName { get; private set; }

    // Constructor utilizado por Entity Framework Core
    private RegisteredLicense()
    {
        LicenseNumber = null!;
        HolderName = null!;
    }

    public RegisteredLicense(string licenseNumber, string holderName)
    {
        if (!LicenseNumberRules.IsValidFormat(licenseNumber))
            throw new ArgumentException(
                "License number must have 4 to 10 digits.",
                nameof(licenseNumber));

        if (string.IsNullOrWhiteSpace(holderName))
            throw new ArgumentException(
                "Holder name cannot be empty.",
                nameof(holderName));

        Id = Guid.NewGuid();
        LicenseNumber = LicenseNumberRules.Normalize(licenseNumber)!;
        HolderName = holderName.Trim();
    }
}
