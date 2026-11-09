using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.Alerting.Domain.Model.Entities;

/// <summary>Records a clinician's observation for an attended alert.</summary>
public class AlertObservation
{
    public const int MaxLength = 1000;

    protected AlertObservation() { }

    public AlertObservation(AlertId alertId, string text, Guid createdBy)
    {
        ArgumentNullException.ThrowIfNull(alertId);
        ArgumentNullException.ThrowIfNull(text);
        if (createdBy == Guid.Empty)
            throw new ArgumentException("Author identifier cannot be empty.", nameof(createdBy));

        var trimmed = text.Trim();
        if (trimmed.Length == 0 || trimmed.EnumerateRunes().Count() > MaxLength)
            throw new ArgumentException($"Observation must contain 1 to {MaxLength} characters.", nameof(text));

        Id = AlertObservationId.New();
        AlertId = alertId;
        Text = trimmed;
        CreatedBy = createdBy;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public AlertObservationId Id { get; private set; } = null!;
    public AlertId AlertId { get; private set; } = null!;
    public string Text { get; private set; } = null!;
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
