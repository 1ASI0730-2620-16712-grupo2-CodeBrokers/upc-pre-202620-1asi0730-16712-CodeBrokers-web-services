using VitaLink.Platform.API.Shared.Domain.Model.ValueObjects;

namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;

/// <summary>
///     Reference limits of the monitoring rules created for a new patient when the family caregiver registers.
/// </summary>
/// <remarks>
///     They are general reference values for the demo, not a medical indication. A measurement outside them
///     raises an alert, and the rules can be changed afterwards from the monitoring rules endpoints.
/// </remarks>
public static class DefaultMonitoringLimits
{
    public sealed record Limit(
        string RecordTypeCode,
        decimal? Min,
        decimal? Max,
        SeverityLevel Severity);

    public static readonly IReadOnlyList<Limit> All =
    [
        new("HEART_RATE", 50m, 110m, SeverityLevel.Medium),
        new("OXYGEN_SATURATION", 92m, null, SeverityLevel.High),
        new("GLUCOSE", 70m, 180m, SeverityLevel.Medium),
        new("BLOOD_PRESSURE", 90m, 150m, SeverityLevel.Medium),
        new("BLOOD_PRESSURE_DIASTOLIC", 50m, 95m, SeverityLevel.Medium)
    ];
}
