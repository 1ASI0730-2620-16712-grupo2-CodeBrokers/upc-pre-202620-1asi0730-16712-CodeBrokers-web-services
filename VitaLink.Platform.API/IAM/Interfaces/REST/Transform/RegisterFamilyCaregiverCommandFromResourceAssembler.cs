using VitaLink.Platform.API.IAM.Application.Results;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Interfaces.REST.Resources;

namespace VitaLink.Platform.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Converts the registration resource into a command and the result into a resource.
/// </summary>
public static class RegisterFamilyCaregiverCommandFromResourceAssembler
{
    public static RegisterFamilyCaregiverCommand ToCommandFromResource(RegisterFamilyCaregiverResource resource)
    {
        var patient = resource.Patient;
        var measurements = resource.Measurements;

        return new RegisterFamilyCaregiverCommand(
            resource.Email,
            resource.Password,
            resource.FirstName,
            resource.LastName,
            new RegistrationPatient(
                patient?.FirstName,
                patient?.LastName,
                patient?.BirthDate,
                patient?.Street,
                patient?.District,
                patient?.City),
            resource.CareProviderId,
            new RegistrationMeasurements(
                measurements?.HeartRate,
                measurements?.OxygenSaturation,
                measurements?.Glucose,
                measurements?.SystolicPressure,
                measurements?.DiastolicPressure),
            resource.AuthorizationAccepted);
    }

    public static CaregiverRegistrationResource ToResourceFromResult(CaregiverRegistrationResult result)
    {
        return new CaregiverRegistrationResource(
            UserResourceFromEntityAssembler.ToResourceFromEntity(result.User),
            result.OlderAdultId,
            result.HealthRecordIds,
            result.AlertsCreated);
    }
}
