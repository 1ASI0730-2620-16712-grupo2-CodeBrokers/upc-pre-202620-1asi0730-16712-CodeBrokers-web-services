using VitaLink.Platform.API.ElderCare.Interfaces.ACL;

namespace VitaLink.Platform.API.IAM.Application.Internal.OutboundServices.ACL;

/// <summary>
///     IAM's view of ElderCare: creates the profile that goes with a new care provider account.
/// </summary>
/// <param name="elderCareContextFacade">Entry point to the ElderCare bounded context.</param>
public class ExternalElderCareService(IElderCareContextFacade elderCareContextFacade)
{
    /// <summary>
    ///     Adds the care provider profile to the current unit of work. The caller commits.
    /// </summary>
    /// <param name="userId">Account the profile belongs to.</param>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="specialty">Medical specialty.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<CareProviderProfileResult> AddCareProviderProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string specialty,
        CancellationToken cancellationToken = default)
    {
        var outcome = await elderCareContextFacade.AddCareProviderProfileAsync(
            userId, firstName, lastName, specialty, cancellationToken);

        return outcome switch
        {
            CareProviderProfileOutcome.Added => CareProviderProfileResult.Added,
            CareProviderProfileOutcome.InvalidData => CareProviderProfileResult.InvalidData,
            CareProviderProfileOutcome.AlreadyExists => CareProviderProfileResult.AlreadyExists,
            _ => throw new InvalidOperationException("Unknown care provider profile outcome.")
        };
    }
}

/// <summary>
///     Result of adding the care provider profile, as IAM sees it.
/// </summary>
public enum CareProviderProfileResult
{
    /// <summary>The profile was added.</summary>
    Added,

    /// <summary>The profile data is not valid.</summary>
    InvalidData,

    /// <summary>The account already has a profile.</summary>
    AlreadyExists
}
