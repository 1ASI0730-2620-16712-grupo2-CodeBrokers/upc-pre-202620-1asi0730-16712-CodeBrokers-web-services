
using VitaLink.ElderCare.Domain.Model.ValueObjects;

namespace VitaLink.ElderCare.Domain.Model.Aggregates;

public class OlderAdult
{
    public OlderAdultId Id { get; private set; }
    public Guid? UserId { get; private set; }
    public FullName FullName { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public Address? Address { get; private set; }
    public EmergencyContact? EmergencyContact { get; private set; }
    public Guid? AssignedProviderId { get; private set; }
    public bool IsMonitoringActive { get; private set; }

    // Constructor para Entity Framework Core
    private OlderAdult()
    {
        Id = null!;
        FullName = null!;
    }

    // Constructor de dominio
    public OlderAdult(
        FullName fullName,
        DateOnly birthDate,
        Address? address)
    {
        ArgumentNullException.ThrowIfNull(fullName);

        if (birthDate == default || birthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException(
                "Birth date must be valid and cannot be in the future.",
                nameof(birthDate));

        Id = OlderAdultId.New();
        FullName = fullName;
        BirthDate = birthDate;
        Address = address;

        UserId = null;
        EmergencyContact = null;
        AssignedProviderId = null;
        IsMonitoringActive = false;
    }
    
    public void LinkCareProvider(Guid providerId)
    {
        if (providerId == Guid.Empty)
            throw new ArgumentException(
                "Care provider ID cannot be empty.",
                nameof(providerId));

        // Idempotency: linking the same provider changes nothing.
        if (AssignedProviderId == providerId)
            return;

        EnsureNoActiveProvider();

        AssignedProviderId = providerId;
    }


    public void UnlinkCareProvider()
    {
        AssignedProviderId = null;
    }

    public bool IsUnlinked()
    {
        return AssignedProviderId is null;
    }

    public int Age()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var age = today.Year - BirthDate.Year;

        if (BirthDate > today.AddYears(-age))
            age--;

        return age;
    }

    private void EnsureNoActiveProvider()
    {
        if (AssignedProviderId is not null)
            throw new InvalidOperationException(
                "Older adult already has an assigned care provider.");
    }
}
