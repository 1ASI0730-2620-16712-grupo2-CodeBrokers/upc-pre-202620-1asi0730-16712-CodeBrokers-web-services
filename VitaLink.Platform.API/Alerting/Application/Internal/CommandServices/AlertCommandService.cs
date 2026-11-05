using Microsoft.EntityFrameworkCore;
using Npgsql;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Application.Internal.OutboundServices.ACL;
using VitaLink.Platform.API.Alerting.Application.Results;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.Alerting.Application.Internal.CommandServices;

/// <summary>
///     Creates and evaluates alerts, enforcing one alert per health record.
/// </summary>
public class AlertCommandService(IAlertRepository alertRepository, IUnitOfWork unitOfWork,
    ExternalMonitoringService monitoringService) : IAlertCommandService
{
    /// <inheritdoc />
    public async Task<Result<Alert, CreateAlertError>> Handle(CreateAlertCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.OlderAdultId == Guid.Empty || command.HealthRecordId == Guid.Empty ||
            command.MonitoringRuleId == Guid.Empty)
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.InvalidReference);

        var healthRecord = await monitoringService.FindHealthRecordAsync(command.HealthRecordId, cancellationToken);
        if (healthRecord is null)
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.HealthRecordNotFound);

        if (healthRecord.OlderAdultId != command.OlderAdultId)
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.InvalidReference);

        var existingAlert = await alertRepository.FindByHealthRecordAsync(command.HealthRecordId, cancellationToken);
        if (existingAlert is not null)
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.AlertAlreadyRaised);

        var alert = new Alert(command);
        return await TrySaveAsync(alert, cancellationToken)
            ? new Result<Alert, CreateAlertError>.Success(alert)
            : new Result<Alert, CreateAlertError>.Failure(CreateAlertError.AlertAlreadyRaised);
    }

    /// <inheritdoc />
    public async Task<Result<AlertEvaluationResult, CreateAlertError>> EvaluateAsync(Guid healthRecordId,
        CancellationToken cancellationToken = default)
    {
        if (healthRecordId == Guid.Empty)
            return new Result<AlertEvaluationResult, CreateAlertError>.Failure(CreateAlertError.InvalidReference);

        var healthRecord = await monitoringService.FindHealthRecordAsync(healthRecordId, cancellationToken);
        if (healthRecord is null)
            return new Result<AlertEvaluationResult, CreateAlertError>.Failure(CreateAlertError.HealthRecordNotFound);

        var existingAlert = await alertRepository.FindByHealthRecordAsync(healthRecordId, cancellationToken);
        if (existingAlert is not null)
            return new Result<AlertEvaluationResult, CreateAlertError>.Success(
                new AlertEvaluationResult(healthRecordId, existingAlert, Created: false));

        if (healthRecord.MeasurementValue is null)
            return new Result<AlertEvaluationResult, CreateAlertError>.Failure(CreateAlertError.UnsupportedHealthRecord);

        var risk = await monitoringService.EvaluateHealthRecordAsync(healthRecordId, cancellationToken);
        // Before exposing this flow, distinguish a missing active rule from an in-range value.
        if (risk is null)
            return new Result<AlertEvaluationResult, CreateAlertError>.Success(
                new AlertEvaluationResult(healthRecordId, Alert: null, Created: false));

        var alert = new Alert(new CreateAlertCommand(risk.OlderAdultId, healthRecordId, risk.MonitoringRuleId,
            risk.Severity));
        if (await TrySaveAsync(alert, cancellationToken))
            return new Result<AlertEvaluationResult, CreateAlertError>.Success(
                new AlertEvaluationResult(healthRecordId, alert, Created: true));

        var concurrentAlert = await alertRepository.FindByHealthRecordAsync(healthRecordId, cancellationToken)
            ?? throw new InvalidOperationException("The concurrent alert could not be retrieved.");
        return new Result<AlertEvaluationResult, CreateAlertError>.Success(
            new AlertEvaluationResult(healthRecordId, concurrentAlert, Created: false));
    }

    private async Task<bool> TrySaveAsync(Alert alert, CancellationToken cancellationToken)
    {
        await alertRepository.AddAsync(alert, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
               {
                   SqlState: PostgresErrorCodes.UniqueViolation,
                   ConstraintName: "i_x_alerts_health_record_id"
               })
        {
            foreach (var entry in exception.Entries)
            {
                if (ReferenceEquals(entry.Entity, alert))
                    entry.State = EntityState.Detached;
            }

            return false;
        }
    }
}
