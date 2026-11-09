using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Application.Internal.CommandServices;
using VitaLink.Platform.API.Alerting.Application.Results;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Patterns;

namespace VitaLink.Platform.API.Notification.Application.Internal.EventHandlers;

/// <summary>
///     Decorates the alert command service to send the email notification of every new alert.
/// </summary>
/// <remarks>
///     The wrapped <see cref="AlertCommandService"/> is not modified. The email is sent only when
///     the alert was created and saved, and the original result is always returned, so a failed
///     notification never changes the outcome of the alert creation.
///     To be replaced by an alert raised event handler once the shared event publisher exists.
/// </remarks>
/// <param name="alertCommandService">The original alert command service.</param>
/// <param name="alertRaisedEmailNotifier">Sends the email notification of a saved alert.</param>
public class NotifyingAlertCommandService(
    AlertCommandService alertCommandService,
    AlertRaisedEmailNotifier alertRaisedEmailNotifier) : IAlertCommandService
{
    /// <inheritdoc />
    public async Task<Result<AlertEvaluationResult, CreateAlertError>> Handle(
        GenerateAlertFromHealthRecordCommand command, CancellationToken cancellationToken = default)
    {
        var result = await alertCommandService.Handle(command, cancellationToken);

        if (result is Result<AlertEvaluationResult, CreateAlertError>.Success
            {
                Value: { Created: true, Alert: { } alert }
            })
            await alertRaisedEmailNotifier.NotifyAsync(
                alert.Id.Value, alert.OlderAdultId, alert.Severity, cancellationToken);

        return result;
    }
}
