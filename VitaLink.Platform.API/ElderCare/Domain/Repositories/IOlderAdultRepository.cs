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

    /// <summary>
    ///     Lists the older adults ordered by last and first name, with optional filters.
    /// </summary>
    /// <param name="search">Text contained in the full name (case insensitive).</param>
    /// <param name="careProviderId">Only those assigned to this care provider.</param>
    /// <param name="isMonitoringActive">Only those with the monitoring active or inactive.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<OlderAdult>> SearchAsync(
        string? search,
        Guid? careProviderId,
        bool? isMonitoringActive,
        CancellationToken cancellationToken = default);
}
