using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Domain.Repositories;

public interface ICareProviderRepository
    : IBaseRepository<CareProvider, CareProviderId>
{
    Task<CareProvider?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByLicenseNumberAsync(
        string licenseNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists the active care providers, optionally only those of one clinic
    ///     or only the independent ones (without a clinic).
    /// </summary>
    Task<IReadOnlyList<CareProvider>> FindActiveAsync(
        Guid? clinicId,
        bool onlyIndependent,
        CancellationToken cancellationToken = default);
}
