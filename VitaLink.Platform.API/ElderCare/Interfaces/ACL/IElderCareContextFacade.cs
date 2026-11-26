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
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<CareProviderProfileOutcome> AddCareProviderProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string specialty,
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
    AlreadyExists
}
