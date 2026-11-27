using VitaLink.Platform.API.ElderCare.Domain.Model.Errors;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.ElderCare.Application.CommandServices;

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

    /// <summary>
    ///     Registers an older adult.
    /// </summary>
    /// <param name="command">The creation command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    ///     The created older adult or the fields that are not valid.
    /// </returns>
    Task<Result<OlderAdult, SaveOlderAdultError>> Handle(
        CreateOlderAdultCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the profile of an older adult.
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    ///     The updated older adult or an expected business error.
    /// </returns>
    Task<Result<OlderAdult, SaveOlderAdultError>> Handle(
        UpdateOlderAdultCommand command,
        CancellationToken cancellationToken = default);
}
