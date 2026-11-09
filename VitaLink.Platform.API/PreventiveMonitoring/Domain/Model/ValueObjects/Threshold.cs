using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

/// <summary>
///     Threshold defining the min and max limits for a monitoring rule.
/// </summary>
public sealed record Threshold
{
    public Threshold(decimal? min, decimal? max)
    {
        if (min is null && max is null)
            throw new ArgumentException("At least one limit (min or max) must be defined.");

        if (min.HasValue && max.HasValue && min > max)
            throw new ArgumentException("Minimum threshold cannot be greater than maximum threshold.");

        Min = min;
        Max = max;
    }

    public decimal? Min { get; }
    public decimal? Max { get; }

    public bool IsViolatedBy(decimal value)
    {
        if (Min.HasValue && value < Min.Value) return true;
        if (Max.HasValue && value > Max.Value) return true;
        return false;
    }
}