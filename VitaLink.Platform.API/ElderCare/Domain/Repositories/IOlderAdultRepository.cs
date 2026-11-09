using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Domain.Repositories;

public interface IOlderAdultRepository
    : IBaseRepository<OlderAdult, OlderAdultId>
{
    Task<IReadOnlyList<OlderAdult>> FindByCareProviderIdAsync(
        CareProviderId providerId,
        CancellationToken cancellationToken = default);
}