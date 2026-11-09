namespace VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>Identifies an alert observation.</summary>
public sealed record AlertObservationId
{
    public AlertObservationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AlertObservationId cannot be empty.", nameof(value));

        Value = value;
    }

    public Guid Value { get; }
    public static AlertObservationId New() => new(Guid.NewGuid());
}
