
using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Alerting.Application.Errors;
using VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;
using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.Alerting.Application.Internal.CommandServices;

/// <summary>
///     Handles alert status changes with optimistic concurrency control.
/// </summary>
public class AlertStatusCommandService(
    IAlertRepository alertRepository,
    IUnitOfWork unitOfWork) : IAlertStatusCommandService
{
    public async Task<Result<Alert, UpdateAlertStatusError>> Handle(
        UpdateAlertStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AlertId == Guid.Empty)
            return Failure(UpdateAlertStatusError.AlertNotFound);

        if (command.ChangedBy == Guid.Empty ||
            string.IsNullOrWhiteSpace(command.Status))
            return Failure(UpdateAlertStatusError.InvalidTransition);

        var alert = await alertRepository.FindByIdAsync(
            new AlertId(command.AlertId),
            cancellationToken);

        if (alert is null)
            return Failure(UpdateAlertStatusError.AlertNotFound);

        if (alert.Version != command.Version)
            return Failure(UpdateAlertStatusError.ConcurrencyConflict);

        var requestedStatus = command.Status.Trim().ToUpperInvariant();

        try
        {
            switch (requestedStatus)
            {
                case "IN_REVIEW":
                    alert.StartReview(command.ChangedBy);
                    break;

                case "ATTENDED":
                    alert.MarkAsAttended(command.ChangedBy);
                    break;

                case "CLOSED":
                    alert.Close(command.ChangedBy);
                    break;

                default:
                    return Failure(UpdateAlertStatusError.InvalidTransition);
            }
        }
        catch (InvalidAlertTransitionException)
        {
            return Failure(UpdateAlertStatusError.InvalidTransition);
        }

        try
        {
            alertRepository.Update(alert);

            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Failure(UpdateAlertStatusError.ConcurrencyConflict);
        }

        return new Result<Alert, UpdateAlertStatusError>.Success(alert);
    }

    private static Result<Alert, UpdateAlertStatusError> Failure(
        UpdateAlertStatusError error)
    {
        return new Result<Alert, UpdateAlertStatusError>.Failure(error);
    }
}
