using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Domain.Repositories;

public interface IClinicRepository
    : IBaseRepository<Clinic, ClinicId>
{
    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Clinic>> ListOrderedByNameAsync(
        CancellationToken cancellationToken = default);
}
