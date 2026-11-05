using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.Alerting.Domain.Services;

/// <summary>
///     Defines operations for changing alert statuses.
/// </summary>
public interface IAlertStatusCommandService
{
    /// <summary>
    ///     Processes an alert status update command.
    /// </summary>
    /// <param name="command">The requested status update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    ///     The updated alert or an expected business error.
    /// </returns>
    Task<Result<Alert, UpdateAlertStatusError>> Handle(
        UpdateAlertStatusCommand command,
        CancellationToken cancellationToken = default);
}