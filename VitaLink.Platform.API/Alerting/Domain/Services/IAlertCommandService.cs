using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.Alerting.Domain.Services;

/// <summary>
///     Creates alerts and reports expected creation failures.
/// </summary>
public interface IAlertCommandService
{
    /// <summary>
    ///     Creates an alert when its references are nonempty and its health record has no alert.
    /// </summary>
    Task<Result<Alert, CreateAlertError>> Handle(CreateAlertCommand command,
        CancellationToken cancellationToken = default);
}
