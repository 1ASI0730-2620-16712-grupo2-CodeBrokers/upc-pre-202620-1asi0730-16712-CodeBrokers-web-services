using Microsoft.EntityFrameworkCore;
using Npgsql;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.Alerting.Application.Internal.CommandServices;

/// <summary>
///     Creates an alert from explicit references, enforcing one alert per health record.
/// </summary>
public class AlertCommandService(IAlertRepository alertRepository, IUnitOfWork unitOfWork) : IAlertCommandService
{
    /// <inheritdoc />
    public async Task<Result<Alert, CreateAlertError>> Handle(CreateAlertCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.OlderAdultId == Guid.Empty || command.HealthRecordId == Guid.Empty ||
            command.MonitoringRuleId == Guid.Empty)
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.InvalidReference);

        var existingAlert = await alertRepository.FindByHealthRecordAsync(command.HealthRecordId, cancellationToken);
        if (existingAlert is not null)
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.AlertAlreadyRaised);

        var alert = new Alert(command);
        await alertRepository.AddAsync(alert, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
               {
                   SqlState: PostgresErrorCodes.UniqueViolation,
                   ConstraintName: "i_x_alerts_health_record_id"
               })
        {
            // A concurrent request may have inserted an alert after the lookup.
            return new Result<Alert, CreateAlertError>.Failure(CreateAlertError.AlertAlreadyRaised);
        }

        return new Result<Alert, CreateAlertError>.Success(alert);
    }
}
