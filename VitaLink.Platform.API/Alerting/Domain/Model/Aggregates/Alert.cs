using VitaLink.Platform.API.Alerting.Domain.Model.Commands;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Services;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root that represents an alert raised when a health record breaks a monitoring rule.
/// </summary>
public class Alert
{
    private readonly List<AlertStatusChange> _statusHistory = [];

    protected Alert()
    {
    }

    public Alert(CreateAlertCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        Id = AlertId.New();
        OlderAdultId = command.OlderAdultId;
        HealthRecordId = command.HealthRecordId;
        MonitoringRuleId = command.MonitoringRuleId;
        Severity = command.Severity;
        Status = AlertStatus.Pending;
        RaisedAt = DateTimeOffset.UtcNow;
        LastStatusChangeAt = RaisedAt;
    }

    public AlertId Id { get; private set; } = null!;

    public Guid OlderAdultId { get; private set; }

    public Guid HealthRecordId { get; private set; }

    public Guid MonitoringRuleId { get; private set; }

    public Guid? AssignedProviderId { get; private set; }

    public SeverityLevel Severity { get; private set; }

    public AlertStatus Status { get; private set; }

    public DateTimeOffset RaisedAt { get; private set; }

    public DateTimeOffset LastStatusChangeAt { get; private set; }

    public IReadOnlyCollection<AlertStatusChange> StatusHistory => _statusHistory;

    /// <summary>
    ///     Indicates whether the alert is still waiting to be reviewed.
    /// </summary>
    public bool IsPending() => Status == AlertStatus.Pending;

    /// <summary>
    ///     Indicates whether the alert has already been reviewed.
    /// </summary>
    public bool IsReviewed() => Status != AlertStatus.Pending;

    /// <summary>
    ///     Returns the user who moved the alert to review, or null if it has not been reviewed.
    /// </summary>
    public Guid? ReviewedBy() =>
        _statusHistory.FirstOrDefault(c => c.ToStatus == AlertStatus.InReview)?.ChangedBy;

    /// <summary>
    ///     Assigns the alert to a care provider.
    /// </summary>
    public void AssignTo(Guid careProviderId)
    {
        if (careProviderId == Guid.Empty)
            throw new ArgumentException("CareProviderId cannot be empty.", nameof(careProviderId));

        AssignedProviderId = careProviderId;
    }

    /// <summary>
    ///     Moves the alert from Pending to InReview.
    /// </summary>
    public void StartReview(Guid changedBy) => ChangeStatus(AlertStatus.InReview, changedBy);

    /// <summary>
    ///     Moves the alert from InReview to Attended.
    /// </summary>
    public void MarkAsAttended(Guid changedBy) => ChangeStatus(AlertStatus.Attended, changedBy);

    /// <summary>
    ///     Moves the alert from Attended to Closed. The alert is never deleted.
    /// </summary>
    public void Close(Guid changedBy) => ChangeStatus(AlertStatus.Closed, changedBy);

    private void ChangeStatus(AlertStatus newStatus, Guid changedBy)
    {
        ValidateTransition(newStatus);
        RegisterStatusChange(newStatus, changedBy);
    }

    private void ValidateTransition(AlertStatus newStatus)
    {
        var isValid = (Status, newStatus) switch
        {
            (AlertStatus.Pending, AlertStatus.InReview) => true,
            (AlertStatus.InReview, AlertStatus.Attended) => true,
            (AlertStatus.Attended, AlertStatus.Closed) => true,
            _ => false
        };

        if (!isValid)
            throw new InvalidAlertTransitionException(Status, newStatus);
    }

    private void RegisterStatusChange(AlertStatus newStatus, Guid changedBy)
    {
        _statusHistory.Add(new AlertStatusChange(Status, newStatus, changedBy));
        Status = newStatus;
        LastStatusChangeAt = DateTimeOffset.UtcNow;
    }
}
