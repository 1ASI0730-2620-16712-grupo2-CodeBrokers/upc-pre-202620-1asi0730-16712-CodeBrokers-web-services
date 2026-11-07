using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

/// <summary>
///     Outcome of evaluating a health record against the active monitoring rules.
/// </summary>
public enum RiskEvaluationOutcome
{
    /// <summary>The record has no measurement value, so no rule can be applied.</summary>
    NotEvaluable,

    /// <summary>There is no active rule for this older adult and record type.</summary>
    NoRuleConfigured,

    /// <summary>At least one active rule exists and the value is inside all limits.</summary>
    WithinLimits,

    /// <summary>The value violates at least one active rule.</summary>
    Violated
}

/// <summary>
///     Result of a risk evaluation. When <see cref="Outcome" /> is <see cref="RiskEvaluationOutcome.Violated" />,
///     <see cref="Rule" /> and <see cref="Severity" /> describe the most severe violated rule.
/// </summary>
public sealed record RiskEvaluationResult
{
    private RiskEvaluationResult(RiskEvaluationOutcome outcome, MonitoringRule? rule)
    {
        Outcome = outcome;
        Rule = rule;
    }

    public RiskEvaluationOutcome Outcome { get; }
    public MonitoringRule? Rule { get; }
    public SeverityLevel? Severity => Rule?.Severity;
    public bool IsViolation => Outcome == RiskEvaluationOutcome.Violated;

    public static RiskEvaluationResult NotEvaluable() => new(RiskEvaluationOutcome.NotEvaluable, null);
    public static RiskEvaluationResult NoRuleConfigured() => new(RiskEvaluationOutcome.NoRuleConfigured, null);
    public static RiskEvaluationResult WithinLimits() => new(RiskEvaluationOutcome.WithinLimits, null);

    public static RiskEvaluationResult Violated(MonitoringRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        return new RiskEvaluationResult(RiskEvaluationOutcome.Violated, rule);
    }
}
