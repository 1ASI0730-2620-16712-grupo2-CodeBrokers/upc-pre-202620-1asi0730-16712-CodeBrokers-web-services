namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;

/// <summary>
///     Catalog entity representing a type of health record measurement.
/// </summary>
public class RecordType
{
    protected RecordType() { }

    public string Code { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string? Unit { get; private set; }
    public decimal? MinValidValue { get; private set; }
    public decimal? MaxValidValue { get; private set; }
    public bool RequiresValue { get; private set; }
    public bool IsActive { get; private set; }
}