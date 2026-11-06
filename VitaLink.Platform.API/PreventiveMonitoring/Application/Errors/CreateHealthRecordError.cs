namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;

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