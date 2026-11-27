namespace VitaLink.Platform.API.ElderCare.Interfaces.ACL;

/// <summary>
///     Entry point other bounded contexts use to work with ElderCare.
/// </summary>
public interface IElderCareContextFacade
{
    /// <summary>
    ///     Adds the profile of a care provider to the current unit of work.
    /// </summary>
    /// <remarks>
    ///     It does not save: the caller commits, so the account and its profile are stored in a single transaction.
    /// </remarks>
    /// <param name="userId">Account the profile belongs to.</param>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="specialty">Medical specialty.</param>
    /// <param name="licenseNumber">Professional license (4 to 10 digits, registered and not used by another account).</param>
    /// <param name="clinicId">Clinic the provider works in, or null when independent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<CareProviderProfileOutcome> AddCareProviderProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string specialty,
        string licenseNumber,
        Guid? clinicId,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Result of adding a care provider profile.
/// </summary>
public enum CareProviderProfileOutcome
{
    /// <summary>The profile was added to the unit of work.</summary>
    Added,

    /// <summary>A name or the specialty is empty or too long.</summary>
    InvalidData,

    /// <summary>The account already has a care provider profile.</summary>
    AlreadyExists,

    /// <summary>The license has a wrong format or is not in the registry.</summary>
    InvalidLicense,

    /// <summary>Another account already uses the license.</summary>
    LicenseAlreadyInUse,

    /// <summary>The clinic does not exist.</summary>
    ClinicNotFound
}
