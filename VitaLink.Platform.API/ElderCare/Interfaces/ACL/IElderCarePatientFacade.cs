namespace VitaLink.Platform.API.ElderCare.Interfaces.ACL;

/// <summary>
///     Lets other bounded contexts register the patient of a family caregiver.
/// </summary>
public interface IElderCarePatientFacade
{
    /// <summary>
    ///     Checks the patient data and the responsible care provider without saving anything.
    /// </summary>
    Task<PatientRegistrationCheck> CheckAsync(
        PatientRegistrationData patient,
        Guid careProviderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves the older adult, assigns the care provider and links the family caregiver.
    /// </summary>
    /// <returns>The identifier of the older adult.</returns>
    Task<Guid> RegisterAsync(
        Guid caregiverUserId,
        string caregiverFirstName,
        string caregiverLastName,
        PatientRegistrationData patient,
        Guid careProviderId,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Data of the patient typed by the family caregiver.
/// </summary>
public record PatientRegistrationData(
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Street,
    string? District,
    string? City);

/// <summary>
///     Result of checking the patient data and the care provider.
/// </summary>
public record PatientRegistrationCheck(
    IReadOnlyList<PatientFieldIssue> Issues,
    CareProviderStatus CareProvider);

/// <summary>
///     A patient field that is not valid.
/// </summary>
public record PatientFieldIssue(
    string Field,
    PatientIssueKind Kind,
    int? Limit = null);

public enum PatientIssueKind
{
    Required,
    TooLong,
    InvalidBirthDate
}

public enum CareProviderStatus
{
    Active,
    NotFound,
    Inactive
}
