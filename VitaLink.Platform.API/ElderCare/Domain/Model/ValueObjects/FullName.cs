namespace VitaLink.ElderCare.Domain.Model.ValueObjects;

public record FullName
{
    public string FirstName { get; init; }
    public string LastName { get; init; }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException(
                "First name cannot be empty.",
                nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException(
                "Last name cannot be empty.",
                nameof(lastName));

        firstName = firstName.Trim();
        lastName = lastName.Trim();

        if (firstName.Length > 100)
            throw new ArgumentException(
                "First name cannot exceed 100 characters.",
                nameof(firstName));

        if (lastName.Length > 100)
            throw new ArgumentException(
                "Last name cannot exceed 100 characters.",
                nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
    }

    public string Display()
    {
        return $"{FirstName} {LastName}";
    }
}