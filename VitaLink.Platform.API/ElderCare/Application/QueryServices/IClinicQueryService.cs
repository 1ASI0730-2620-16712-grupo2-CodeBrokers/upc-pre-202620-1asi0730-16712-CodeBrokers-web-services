using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Queries;

namespace VitaLink.Platform.API.ElderCare.Application.QueryServices;

/// <summary>
///     Defines query operations for clinics.
/// </summary>
public interface IClinicQueryService
{
    /// <summary>
    ///     Lists the clinics ordered by name.
    /// </summary>
    Task<IReadOnlyList<Clinic>> Handle(
        GetClinicsQuery query,
        CancellationToken cancellationToken = default);
}
