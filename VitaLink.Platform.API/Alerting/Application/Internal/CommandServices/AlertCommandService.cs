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
    public async Task<Result<AlertEvaluationResult, CreateAlertError>> Handle(GenerateAlertFromHealthRecordCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var healthRecordId = command.HealthRecordId;
        if (healthRecordId == Guid.Empty)
            return new Result<AlertEvaluationResult, CreateAlertError>.Failure(CreateAlertError.InvalidReference);

        var existingAlert = await alertRepository.FindByHealthRecordAsync(healthRecordId, cancellationToken);
        if (existingAlert is not null)
            return new Result<AlertEvaluationResult, CreateAlertError>.Success(
                new AlertEvaluationResult(healthRecordId, existingAlert, Created: false));

        var risk = await monitoringService.EvaluateHealthRecordAsync(healthRecordId, cancellationToken);
        if (risk is null)
            return new Result<AlertEvaluationResult, CreateAlertError>.Failure(CreateAlertError.HealthRecordNotFound);

        if (risk.Outcome is HealthRecordRiskOutcome.NotEvaluable or HealthRecordRiskOutcome.NoRuleConfigured
            or HealthRecordRiskOutcome.WithinLimits)
            return new Result<AlertEvaluationResult, CreateAlertError>.Success(
                new AlertEvaluationResult(healthRecordId, Alert: null, Created: false));

        if (risk.Outcome != HealthRecordRiskOutcome.Violated || risk.MonitoringRuleId is not Guid ruleId ||
            risk.Severity is not { } severity)
            throw new InvalidOperationException("The risk evaluation did not identify a violated rule and severity.");

        var alert = new Alert(new CreateAlertCommand(risk.OlderAdultId, healthRecordId, ruleId, severity));
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
