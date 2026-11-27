using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

public class CareProvider
{
    public CareProviderId Id { get; private set; }
    public Guid UserId { get; private set; }
    public FullName FullName { get; private set; }
    public Specialty Specialty { get; private set; }
    public string? Institution { get; private set; }
    public string? LicenseNumber { get; private set; }
    public Guid? ClinicId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Constructor utilizado por Entity Framework Core
    private CareProvider()
    {
        Id = null!;
        FullName = null!;
        Specialty = null!;
    }

    // Constructor para registrar un proveedor de salud
    public CareProvider(
        Guid userId,
        FullName fullName,
        Specialty specialty)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));

        ArgumentNullException.ThrowIfNull(fullName);
        ArgumentNullException.ThrowIfNull(specialty);

        Id = CareProviderId.New();
        UserId = userId;
        FullName = fullName;
        Specialty = specialty;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    // Constructor para registrar un proveedor con licencia y clinica (sin clinica = independiente)
    public CareProvider(
        Guid userId,
        FullName fullName,
        Specialty specialty,
        string licenseNumber,
        Guid? clinicId)
        : this(userId, fullName, specialty)
    {
        if (!LicenseNumberRules.IsValidFormat(licenseNumber))
            throw new ArgumentException(
                "License number must have 4 to 10 digits.",
                nameof(licenseNumber));

        if (clinicId == Guid.Empty)
            throw new ArgumentException(
                "Clinic ID cannot be empty.",
                nameof(clinicId));

        LicenseNumber = LicenseNumberRules.Normalize(licenseNumber);
        ClinicId = clinicId;
    }

    public void UpdateSpecialty(Specialty specialty)
    {
        ArgumentNullException.ThrowIfNull(specialty);

        Specialty = specialty;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
