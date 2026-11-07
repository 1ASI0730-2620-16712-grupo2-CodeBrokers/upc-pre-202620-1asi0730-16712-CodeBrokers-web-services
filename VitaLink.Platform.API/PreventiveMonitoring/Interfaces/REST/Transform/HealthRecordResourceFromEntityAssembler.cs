using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;

public static class HealthRecordResourceFromEntityAssembler
{
    public static HealthRecordResource ToResourceFromEntity(HealthRecord entity)
    {
        var sourceString = entity.Source switch
        {
            RecordSource.OlderAdult => "OLDER_ADULT",
            RecordSource.FamilyCaregiver => "FAMILY_CAREGIVER",
            RecordSource.CareProvider => "CARE_PROVIDER",
            _ => entity.Source.ToString()
        };

        return new HealthRecordResource(
            entity.Id.Value,
            entity.OlderAdultId,
            entity.RecordTypeCode,
            entity.Measurement?.Value,
            entity.Note,
            sourceString,
            entity.RecordedBy,
            entity.RecordedAt
        );
    }
}