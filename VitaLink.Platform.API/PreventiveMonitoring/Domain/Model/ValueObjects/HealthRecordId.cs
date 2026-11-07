namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

/// <summary>
///     Unique identifier of a health record.
/// </summary>
public sealed record HealthRecordId
{
    public HealthRecordId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("HealthRecordId cannot be empty.", nameof(value));

        Value = value;
    }

    public Guid Value { get; }

    public static HealthRecordId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}