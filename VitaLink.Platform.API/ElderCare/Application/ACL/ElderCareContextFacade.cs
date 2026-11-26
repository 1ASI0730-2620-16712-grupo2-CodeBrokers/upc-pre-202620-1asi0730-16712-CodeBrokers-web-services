using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.ElderCare.Interfaces.ACL;

namespace VitaLink.Platform.API.ElderCare.Application.ACL;

/// <inheritdoc />
public class ElderCareContextFacade(ICareProviderRepository careProviderRepository) : IElderCareContextFacade
{
    /// <inheritdoc />
    public async Task<CareProviderProfileOutcome> AddCareProviderProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string specialty,
        CancellationToken cancellationToken = default)
    {
        CareProvider careProvider;
        try
        {
            careProvider = new CareProvider(userId, new FullName(firstName, lastName), new Specialty(specialty));
        }
        catch (ArgumentException)
        {
            return CareProviderProfileOutcome.InvalidData;
        }

        if (await careProviderRepository.FindByUserIdAsync(userId, cancellationToken) is not null)
            return CareProviderProfileOutcome.AlreadyExists;

        await careProviderRepository.AddAsync(careProvider, cancellationToken);
        return CareProviderProfileOutcome.Added;
    }
}
