using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a health record for an older adult.
/// </summary>
public class HealthRecord
{
    protected HealthRecord() { }

    public HealthRecord(CreateHealthRecordCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.MeasurementValue is null && string.IsNullOrWhiteSpace(command.Note))
            throw new ArgumentException("A health record must have either a measurement value or a note.");

        Id = HealthRecordId.New();
        OlderAdultId = command.OlderAdultId;
        RecordTypeCode = command.RecordTypeCode;
        
        if (command.MeasurementValue.HasValue)
            Measurement = new Measurement(command.MeasurementValue.Value);

        Note = command.Note;
        Source = command.Source;
        RecordedBy = command.RecordedBy;
        RecordedAt = DateTimeOffset.UtcNow;
    }

    public HealthRecordId Id { get; private set; } = null!;
    public Guid OlderAdultId { get; private set; }
    public string RecordTypeCode { get; private set; } = null!;
    public Measurement? Measurement { get; private set; }
    public string? Note { get; private set; }
    public RecordSource Source { get; private set; }
    public Guid RecordedBy { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }

    public void AttachNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Note cannot be empty.", nameof(note));
        
        Note = note;
    }

    public string Summary()
    {
        var valueStr = Measurement != null ? Measurement.Value.ToString() : "No value";
        return $"{RecordTypeCode}: {valueStr} (Note: {Note ?? "None"})";
    }
}