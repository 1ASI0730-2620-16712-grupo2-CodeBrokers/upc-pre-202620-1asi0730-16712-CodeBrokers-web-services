using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a health monitoring rule for an older adult.
/// </summary>
public class MonitoringRule
{
    protected MonitoringRule() { }

    public MonitoringRule(CreateMonitoringRuleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.RecordTypeCode))
            throw new ArgumentException("Record type code cannot be empty.", nameof(command));

        Id = MonitoringRuleId.New();
        OlderAdultId = command.OlderAdultId;
        RecordTypeCode = command.RecordTypeCode;
        Threshold = new Threshold(command.MinThreshold, command.MaxThreshold);
        Severity = command.Severity;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public MonitoringRuleId Id { get; private set; } = null!;
    public Guid OlderAdultId { get; private set; }
    public string RecordTypeCode { get; private set; } = null!;
    public Threshold Threshold { get; private set; } = null!;
    public SeverityLevel Severity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}