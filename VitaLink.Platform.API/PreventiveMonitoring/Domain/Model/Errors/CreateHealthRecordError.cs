namespace VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Errors;

public enum CreateHealthRecordError
{
    InvalidReference,
    InvalidRecordType,
    ValueRequired,
    ValueOutOfRange,
    InvalidUnit,
    InvalidNote,
    EmptyRecord
}