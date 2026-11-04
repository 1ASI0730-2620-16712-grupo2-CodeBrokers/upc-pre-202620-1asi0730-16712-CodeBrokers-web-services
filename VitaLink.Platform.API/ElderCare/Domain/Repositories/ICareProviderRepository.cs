using VitaLink.ElderCare.Domain.Model.Aggregates;
using VitaLink.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.ElderCare.Domain.Repositories;

public interface ICareProviderRepository
    : IBaseRepository<CareProvider, CareProviderId>
{
    Task<CareProvider?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}