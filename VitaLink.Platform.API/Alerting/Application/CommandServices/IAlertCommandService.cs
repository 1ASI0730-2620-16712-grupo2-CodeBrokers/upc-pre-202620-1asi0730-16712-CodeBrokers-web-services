using VitaLink.Platform.API.Alerting.Domain.Model.Errors;
using VitaLink.Platform.API.Alerting.Application.Results;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Shared.Application.Model;

namespace VitaLink.Platform.API.Alerting.Application.CommandServices;

/// <summary>
///     Creates alerts and reports expected creation failures.
/// </summary>
public interface IAlertCommandService
{
    /// <summary>
    ///     Returns an existing alert or evaluates a health record to decide whether to create one.
    /// </summary>
    Task<Result<AlertEvaluationResult, CreateAlertError>> Handle(GenerateAlertFromHealthRecordCommand command,
        CancellationToken cancellationToken = default);
}
