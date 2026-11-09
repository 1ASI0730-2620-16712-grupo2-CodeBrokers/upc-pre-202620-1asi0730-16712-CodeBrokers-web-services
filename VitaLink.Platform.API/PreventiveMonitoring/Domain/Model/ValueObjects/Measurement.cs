namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

/// <summary>
///     Measurement value for a health record.
/// </summary>
public sealed record Measurement
{
    public Measurement(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Measurement value cannot be negative.", nameof(value));

        Value = value;
    }

    public decimal Value { get; }
}