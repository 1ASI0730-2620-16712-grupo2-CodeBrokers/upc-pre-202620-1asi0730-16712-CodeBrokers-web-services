using Microsoft.EntityFrameworkCore;
using Npgsql;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.Alerting.Application.Internal.CommandServices;

/// <summary>Validates and persists one observation per attended alert.</summary>
public class AlertObservationCommandService(IAlertRepository alerts, IAlertObservationRepository observations,
    IUnitOfWork unitOfWork) : IAlertObservationCommandService
{
    public async Task<Result<AlertObservation, CreateAlertObservationError>> Handle(CreateAlertObservationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AlertId == Guid.Empty || command.CreatedBy == Guid.Empty)
            return Failure(CreateAlertObservationError.InvalidReference);

        if (command.Text.Trim().EnumerateRunes().Count() > AlertObservation.MaxLength)
            return Failure(CreateAlertObservationError.TextTooLong);

        var alert = await alerts.FindByIdAsync(new AlertId(command.AlertId), cancellationToken);
        if (alert is null)
            return Failure(CreateAlertObservationError.AlertNotFound);
        if (alert.Status != AlertStatus.Attended)
            return Failure(CreateAlertObservationError.AlertNotAttended);
        if (await observations.FindByAlertAsync(command.AlertId, cancellationToken) is not null)
            return Failure(CreateAlertObservationError.ObservationAlreadyExists);

        var observation = new AlertObservation(alert.Id, command.Text, command.CreatedBy);
        await observations.AddAsync(observation, cancellationToken);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
               {
                   SqlState: PostgresErrorCodes.UniqueViolation,
                   ConstraintName: "uq_alert_observations_alert_id"
               })
        {
            return Failure(CreateAlertObservationError.ObservationAlreadyExists);
        }

        return new Result<AlertObservation, CreateAlertObservationError>.Success(observation);
    }

    private static Result<AlertObservation, CreateAlertObservationError> Failure(CreateAlertObservationError error) =>
        new Result<AlertObservation, CreateAlertObservationError>.Failure(error);
}
