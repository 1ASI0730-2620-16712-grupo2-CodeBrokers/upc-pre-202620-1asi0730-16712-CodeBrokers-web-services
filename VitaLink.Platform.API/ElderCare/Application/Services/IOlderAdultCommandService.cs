using VitaLink.ElderCare.Application.Errors;
using VitaLink.ElderCare.Domain.Model.Aggregates;
using VitaLink.ElderCare.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.ElderCare.Application.Services;

/// <summary>
///     Defines command operations for managing older adults.
/// </summary>
public interface IOlderAdultCommandService
{
    /// <summary>
    ///     Links a care provider to an older adult.
    /// </summary>
    /// <param name="command">The linking command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    ///     The updated older adult or an expected business error.
    /// </returns>
    Task<Result<OlderAdult, LinkCareProviderError>> Handle(
        LinkCareProviderCommand command,
        CancellationToken cancellationToken = default);
}