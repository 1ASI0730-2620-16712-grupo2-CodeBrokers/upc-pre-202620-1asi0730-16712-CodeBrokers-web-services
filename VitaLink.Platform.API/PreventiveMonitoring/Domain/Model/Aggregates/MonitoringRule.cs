using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

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

        if (string.IsNullOrWhiteSpace(command.SeverityLevel))
            throw new ArgumentException("Severity level cannot be empty.", nameof(command));

        Id = MonitoringRuleId.New();
        OlderAdultId = command.OlderAdultId;
        RecordTypeCode = command.RecordTypeCode;
        Threshold = new Threshold(command.MinThreshold, command.MaxThreshold);
        SeverityLevel = command.SeverityLevel;
        Description = command.Description;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public MonitoringRuleId Id { get; private set; } = null!;
    public Guid OlderAdultId { get; private set; }
    public string RecordTypeCode { get; private set; } = null!;
    public Threshold Threshold { get; private set; } = null!;
    public string SeverityLevel { get; private set; } = null!;
    public string Description { get; private set; } = null!;
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