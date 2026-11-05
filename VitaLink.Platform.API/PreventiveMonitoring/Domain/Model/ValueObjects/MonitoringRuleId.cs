using System;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

/// <summary>
///     Unique identifier of a monitoring rule.
/// </summary>
public sealed record MonitoringRuleId
{
    public MonitoringRuleId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("MonitoringRuleId cannot be empty.", nameof(value));

        Value = value;
    }

    public Guid Value { get; }

    public static MonitoringRuleId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}