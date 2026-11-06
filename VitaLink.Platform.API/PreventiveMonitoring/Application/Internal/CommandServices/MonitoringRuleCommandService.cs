using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;

/// <summary>
///     Command service implementation for monitoring rules.
/// </summary>
public class MonitoringRuleCommandService : IMonitoringRuleCommandService
{
    private readonly IMonitoringRuleRepository _monitoringRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MonitoringRuleCommandService(
        IMonitoringRuleRepository monitoringRuleRepository,
        IUnitOfWork unitOfWork)
    {
        _monitoringRuleRepository = monitoringRuleRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<MonitoringRule, CreateMonitoringRuleError>> Handle(
        CreateMonitoringRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.OlderAdultId == Guid.Empty)
            return Failure(CreateMonitoringRuleError.InvalidReference);

        if (string.IsNullOrWhiteSpace(command.RecordTypeCode))
            return Failure(CreateMonitoringRuleError.InvalidRecordType);

        if ((command.MinThreshold is null && command.MaxThreshold is null) ||
            command.MinThreshold > command.MaxThreshold)
            return Failure(CreateMonitoringRuleError.InvalidThreshold);

        var rule = new MonitoringRule(command);
        await _monitoringRuleRepository.AddAsync(rule, cancellationToken);

        try
        {
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
               {
                   SqlState: PostgresErrorCodes.UniqueViolation,
                   ConstraintName: "uq_mr_active_rule"
               })
        {
            return Failure(CreateMonitoringRuleError.DuplicateActiveRule);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
               {
                   SqlState: PostgresErrorCodes.ForeignKeyViolation,
                   ConstraintName: "f_k_monitoring_rules_record_types_record_type_code"
               })
        {
            // The record type code does not exist in monitoring.record_types.
            return Failure(CreateMonitoringRuleError.InvalidRecordType);
        }

        return new Result<MonitoringRule, CreateMonitoringRuleError>.Success(rule);
    }

    private static Result<MonitoringRule, CreateMonitoringRuleError> Failure(CreateMonitoringRuleError error) =>
        new Result<MonitoringRule, CreateMonitoringRuleError>.Failure(error);
}