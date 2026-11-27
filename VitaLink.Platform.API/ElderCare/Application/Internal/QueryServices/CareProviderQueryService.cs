using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Application.Results;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Application.Internal.QueryServices;

/// <summary>
///     Handles the queries on care providers.
/// </summary>
public class CareProviderQueryService(
    ICareProviderRepository careProviderRepository,
    IClinicRepository clinicRepository) : ICareProviderQueryService
{
    public async Task<IReadOnlyList<CareProviderSummary>> Handle(
        GetCareProvidersQuery query,
        CancellationToken cancellationToken = default)
    {
        var providers = await careProviderRepository.FindActiveAsync(
            query.ClinicId,
            query.OnlyIndependent,
            cancellationToken);

        var clinics = await clinicRepository.ListOrderedByNameAsync(cancellationToken);
        var clinicNames = clinics.ToDictionary(clinic => clinic.Id.Value, clinic => clinic.Name);

        return providers
            .Select(provider => new CareProviderSummary(
                provider,
                provider.ClinicId is { } clinicId && clinicNames.TryGetValue(clinicId, out var name)
                    ? name
                    : null))
            .ToList();
    }
}
