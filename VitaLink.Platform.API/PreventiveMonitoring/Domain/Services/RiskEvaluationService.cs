using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;

/// <summary>
///     Domain service to evaluate health records against monitoring rules.
/// </summary>
public class RiskEvaluationService
{
    public bool EvaluateRisk(HealthRecord record, MonitoringRule rule, out string? severityLevel)
    {
        severityLevel = null;

        if (record is null || rule is null || !rule.IsActive)
            return false;

        if (record.RecordTypeCode != rule.RecordTypeCode)
            return false;

        if (record.Measurement?.Value is decimal value && rule.Threshold.IsViolatedBy(value))
        {
            severityLevel = rule.SeverityLevel;
            return true;
        }

        return false;
    }
}