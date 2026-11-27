using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Application.Internal.QueryServices;

/// <summary>
///     Handles the queries on clinics.
/// </summary>
public class ClinicQueryService(IClinicRepository clinicRepository) : IClinicQueryService
{
    public async Task<IReadOnlyList<Clinic>> Handle(
        GetClinicsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await clinicRepository.ListOrderedByNameAsync(cancellationToken);
    }
}
