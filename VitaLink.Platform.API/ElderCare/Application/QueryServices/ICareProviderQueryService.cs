using VitaLink.Platform.API.ElderCare.Application.Results;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

namespace VitaLink.Platform.API.ElderCare.Application.QueryServices;

/// <summary>
///     Defines query operations for care providers.
/// </summary>
public interface ICareProviderQueryService
{
    /// <summary>
    ///     Lists the active care providers, by clinic or independent ones.
    /// </summary>
    Task<IReadOnlyList<CareProviderSummary>> Handle(
        GetCareProvidersQuery query,
        CancellationToken cancellationToken = default);
}
