using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.Alerting.Interfaces.REST.Assemblers;

/// <summary>
///     Converts alert status history entities into REST resources.
/// </summary>
public static class AlertStatusChangeResourceFromEntityAssembler
{
    public static AlertStatusChangeResource ToResourceFromEntity(
        AlertStatusChange entity)
    {
        return new AlertStatusChangeResource(
            entity.Id.Value,
            ToApiStatus(entity.FromStatus),
            ToApiStatus(entity.ToStatus),
            entity.ChangedBy,
            entity.ChangedAt);
    }

    private static string ToApiStatus(AlertStatus status)
    {
        return status switch
        {
            AlertStatus.Pending => "PENDING",
            AlertStatus.InReview => "IN_REVIEW",
            AlertStatus.Attended => "ATTENDED",
            AlertStatus.Closed => "CLOSED",
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }
}