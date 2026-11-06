using System;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.PreventiveMonitoring.Interfaces.REST.Transform;

public static class CreateHealthRecordCommandFromResourceAssembler
{
    public static CreateHealthRecordCommand ToCommandFromResource(Guid olderAdultId, CreateHealthRecordResource resource)
    {
        var source = Enum.Parse<RecordSource>(resource.Source!.Replace("_", string.Empty), ignoreCase: true);

        return new CreateHealthRecordCommand(
            olderAdultId,
            resource.RecordTypeCode!,
            resource.MeasurementValue,
            resource.Note,
            source,
            resource.RecordedBy.GetValueOrDefault()
        );
    }
}