using VitaLink.Platform.API.Alerting.Interfaces.ACL;
using VitaLink.Platform.API.ElderCare.Interfaces.ACL;
using VitaLink.Platform.API.IAM.Domain.Model.Commands;
using VitaLink.Platform.API.IAM.Domain.Model.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;

namespace VitaLink.Platform.API.IAM.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Translates the calls that the caregiver registration makes to ElderCare, Preventive Monitoring and Alerting.
/// </summary>
public class ExternalCaregiverRegistrationService(
    IElderCarePatientFacade elderCarePatientFacade,
    IMonitoringRegistrationFacade monitoringRegistrationFacade,
    IAlertingContextFacade alertingContextFacade)
{
    /// <summary>
    ///     Checks the patient, the care provider and the measurements; saves nothing.
    /// </summary>
    public async Task<IReadOnlyList<RegistrationFieldError>> CheckAsync(
        RegistrationPatient patient,
        Guid? careProviderId,
        RegistrationMeasurements measurements,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<RegistrationFieldError>();

        var patientCheck = await elderCarePatientFacade.CheckAsync(
            ToPatientData(patient),
            careProviderId ?? Guid.Empty,
            cancellationToken);

        foreach (var issue in patientCheck.Issues)
        {
            var code = issue.Kind switch
            {
                PatientIssueKind.Required => RegistrationFieldCode.Required,
                PatientIssueKind.TooLong => RegistrationFieldCode.TooLong,
                PatientIssueKind.InvalidBirthDate => RegistrationFieldCode.InvalidBirthDate,
                _ => throw new InvalidOperationException("Unknown patient issue.")
            };

            errors.Add(new RegistrationFieldError(issue.Field, code, issue.Limit));
        }

        if (careProviderId is null)
            errors.Add(new RegistrationFieldError("careProviderId", RegistrationFieldCode.Required));
        else if (patientCheck.CareProvider == CareProviderStatus.NotFound)
            errors.Add(new RegistrationFieldError("careProviderId", RegistrationFieldCode.CareProviderNotFound));
        else if (patientCheck.CareProvider == CareProviderStatus.Inactive)
            errors.Add(new RegistrationFieldError("careProviderId", RegistrationFieldCode.CareProviderInactive));

        var measurementIssues = await monitoringRegistrationFacade.ValidateAsync(
            ToInitialMeasurements(measurements),
            cancellationToken);

        foreach (var issue in measurementIssues)
        {
            var code = issue.Kind switch
            {
                MeasurementIssueKind.Required => RegistrationFieldCode.Required,
                MeasurementIssueKind.OutOfRange => RegistrationFieldCode.OutOfRange,
                MeasurementIssueKind.PressureOrder => RegistrationFieldCode.PressureOrder,
                _ => throw new InvalidOperationException("Unknown measurement issue.")
            };

            errors.Add(new RegistrationFieldError(issue.Field, code, null, issue.Min, issue.Max));
        }

        return errors;
    }

    /// <summary>
    ///     Saves the patient and links the caregiver.
    /// </summary>
    public Task<Guid> RegisterPatientAsync(
        Guid caregiverUserId,
        string caregiverFirstName,
        string caregiverLastName,
        RegistrationPatient patient,
        Guid careProviderId,
        CancellationToken cancellationToken = default)
    {
        return elderCarePatientFacade.RegisterAsync(
            caregiverUserId,
            caregiverFirstName,
            caregiverLastName,
            ToPatientData(patient),
            careProviderId,
            cancellationToken);
    }

    /// <summary>
    ///     Saves the first measurements and the default monitoring rules of the patient.
    /// </summary>
    public Task<IReadOnlyList<Guid>> SaveMeasurementsAsync(
        Guid olderAdultId,
        Guid recordedBy,
        RegistrationMeasurements measurements,
        CancellationToken cancellationToken = default)
    {
        return monitoringRegistrationFacade.SaveAsync(
            olderAdultId,
            recordedBy,
            ToInitialMeasurements(measurements),
            cancellationToken);
    }

    /// <summary>
    ///     Evaluates the saved measurements; the alerts that are raised send their email.
    /// </summary>
    public Task<int> GenerateAlertsAsync(
        IEnumerable<Guid> healthRecordIds,
        CancellationToken cancellationToken = default)
    {
        return alertingContextFacade.GenerateAlertsAsync(healthRecordIds, cancellationToken);
    }

    private static PatientRegistrationData ToPatientData(RegistrationPatient patient)
    {
        return new PatientRegistrationData(
            patient.FirstName,
            patient.LastName,
            patient.BirthDate,
            patient.Street,
            patient.District,
            patient.City);
    }

    private static InitialMeasurements ToInitialMeasurements(RegistrationMeasurements measurements)
    {
        return new InitialMeasurements(
            measurements.HeartRate,
            measurements.OxygenSaturation,
            measurements.Glucose,
            measurements.SystolicPressure,
            measurements.DiastolicPressure);
    }
}
