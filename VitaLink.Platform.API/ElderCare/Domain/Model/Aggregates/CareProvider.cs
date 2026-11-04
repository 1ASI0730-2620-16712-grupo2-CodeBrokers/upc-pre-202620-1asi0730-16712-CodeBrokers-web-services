using VitaLink.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.ElderCare.Domain.Model.Aggregates;

public class CareProvider
{
    public CareProviderId Id { get; private set; }
    public Guid UserId { get; private set; }
    public FullName FullName { get; private set; }
    public Specialty Specialty { get; private set; }
    public string? Institution { get; private set; }
    public bool IsActive { get; private set; }

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