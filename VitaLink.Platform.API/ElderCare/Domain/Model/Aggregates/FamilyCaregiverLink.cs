namespace VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

/// <summary>
///     Relationship between a family caregiver account and an older adult they take care of.
/// </summary>
/// <remarks>
///     It is created when a family caregiver registers, and it keeps the caregiver name because the account
///     (IAM) only stores the credentials. It is the base to show each caregiver only their own patients.
/// </remarks>
public class FamilyCaregiverLink
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid OlderAdultId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // Constructor for Entity Framework Core
    private FamilyCaregiverLink()
    {
        FirstName = null!;
        LastName = null!;
    }

    public FamilyCaregiverLink(
        Guid userId,
        Guid olderAdultId,
        string firstName,
        string lastName)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (olderAdultId == Guid.Empty)
            throw new ArgumentException("Older adult ID cannot be empty.", nameof(olderAdultId));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        Id = Guid.NewGuid();
        UserId = userId;
        OlderAdultId = olderAdultId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
