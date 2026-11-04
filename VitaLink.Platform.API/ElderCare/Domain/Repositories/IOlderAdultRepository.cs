using VitaLink.ElderCare.Domain.Model.Aggregates;
using VitaLink.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.ElderCare.Domain.Repositories;

public interface IOlderAdultRepository
    : IBaseRepository<OlderAdult, OlderAdultId>
{
    Task<IReadOnlyList<OlderAdult>> FindByCareProviderIdAsync(
        CareProviderId providerId,
        CancellationToken cancellationToken = default);
}