using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

namespace VitaLink.Platform.API.ElderCare.Application.QueryServices;

/// <summary>
///     Defines query operations for older adults.
/// </summary>
public interface IOlderAdultQueryService
{
    /// <summary>
    ///     Lists the older adults that match the filters.
    /// </summary>
    Task<IReadOnlyList<OlderAdult>> Handle(
        GetOlderAdultsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets one older adult, or <c>null</c> when it does not exist.
    /// </summary>
    Task<OlderAdult?> Handle(
        GetOlderAdultByIdQuery query,
        CancellationToken cancellationToken = default);
}
