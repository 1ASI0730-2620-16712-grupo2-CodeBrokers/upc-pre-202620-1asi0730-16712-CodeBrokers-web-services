using System;
using System.ComponentModel.DataAnnotations;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

public record CreateHealthRecordResource(
    [Required(ErrorMessage = "HealthRecordInvalidRecordType")] string? RecordTypeCode,
    decimal? MeasurementValue,
    string? Unit,
    string? Note,
    [Required(ErrorMessage = "HealthRecordInvalidSource")]
    [RegularExpression("^(OLDER_ADULT|FAMILY_CAREGIVER|CARE_PROVIDER)$", ErrorMessage = "HealthRecordInvalidSource")] string? Source,
    [Required(ErrorMessage = "HealthRecordInvalidReference")] Guid? RecordedBy);