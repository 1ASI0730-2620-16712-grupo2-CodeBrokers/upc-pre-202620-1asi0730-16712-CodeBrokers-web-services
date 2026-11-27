using VitaLink.Platform.API.ElderCare.Application.QueryServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Application.Internal.QueryServices;

/// <summary>
///     Handles the queries of older adults.
/// </summary>
public class OlderAdultQueryService(
    IOlderAdultRepository olderAdultRepository) : IOlderAdultQueryService
{
    public async Task<IReadOnlyList<OlderAdult>> Handle(
        GetOlderAdultsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await olderAdultRepository.SearchAsync(
            query.Search,
            query.CareProviderId,
            query.IsMonitoringActive,
            cancellationToken);
    }

    public async Task<OlderAdult?> Handle(
        GetOlderAdultByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await olderAdultRepository.FindByIdAsync(
            new OlderAdultId(query.OlderAdultId),
            cancellationToken);
    }
}
