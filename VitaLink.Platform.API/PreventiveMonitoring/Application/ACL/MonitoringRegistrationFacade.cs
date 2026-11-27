using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.ValueObjects;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Services;
using VitaLink.Platform.API.PreventiveMonitoring.Interfaces.ACL;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.ACL;

/// <summary>
///     Saves the first measurements and the default monitoring rules of a registered patient.
/// </summary>
public class MonitoringRegistrationFacade(
    IRecordTypeRepository recordTypeRepository,
    IHealthRecordRepository healthRecordRepository,
    IMonitoringRuleRepository monitoringRuleRepository,
    IUnitOfWork unitOfWork) : IMonitoringRegistrationFacade
{
    // Request field name -> record type code, in the order the records are saved.
    private static readonly (string Field, string Code)[] Signals =
    [
        ("measurements.heartRate", "HEART_RATE"),
        ("measurements.oxygenSaturation", "OXYGEN_SATURATION"),
        ("measurements.glucose", "GLUCOSE"),
        ("measurements.systolicPressure", "BLOOD_PRESSURE"),
        ("measurements.diastolicPressure", "BLOOD_PRESSURE_DIASTOLIC")
    ];

    public async Task<IReadOnlyList<MeasurementIssue>> ValidateAsync(
        InitialMeasurements measurements,
        CancellationToken cancellationToken = default)
    {
        var issues = new List<MeasurementIssue>();

        foreach (var (field, code) in Signals)
        {
            var value = ValueOf(measurements, code);

            if (value is null)
            {
                issues.Add(new MeasurementIssue(field, MeasurementIssueKind.Required));
                continue;
            }

            var recordType = await GetRecordTypeAsync(code, cancellationToken);

            if (value < (recordType.MinValidValue ?? 0m)
                || value > (recordType.MaxValidValue ?? 99999999.99m))
                issues.Add(new MeasurementIssue(
                    field,
                    MeasurementIssueKind.OutOfRange,
                    recordType.MinValidValue,
                    recordType.MaxValidValue));
        }

        // The systolic pressure is always higher than the diastolic one.
        if (measurements.SystolicPressure is { } systolic
            && measurements.DiastolicPressure is { } diastolic
            && systolic <= diastolic
            && issues.All(i => i.Field is not ("measurements.systolicPressure" or "measurements.diastolicPressure")))
            issues.Add(new MeasurementIssue(
                "measurements.diastolicPressure",
                MeasurementIssueKind.PressureOrder));

        return issues;
    }

    public async Task<IReadOnlyList<Guid>> SaveAsync(
        Guid olderAdultId,
        Guid recordedBy,
        InitialMeasurements measurements,
        CancellationToken cancellationToken = default)
    {
        var recordIds = new List<Guid>();

        foreach (var (_, code) in Signals)
        {
            var recordType = await GetRecordTypeAsync(code, cancellationToken);

            var record = new HealthRecord(new CreateHealthRecordCommand(
                olderAdultId,
                code,
                ValueOf(measurements, code),
                recordType.Unit,
                null,
                RecordSource.FamilyCaregiver,
                recordedBy));

            await healthRecordRepository.AddAsync(record, cancellationToken);
            recordIds.Add(record.Id.Value);
        }

        foreach (var limit in DefaultMonitoringLimits.All)
        {
            await monitoringRuleRepository.AddAsync(
                new MonitoringRule(new CreateMonitoringRuleCommand(
                    olderAdultId,
                    limit.RecordTypeCode,
                    limit.Min,
                    limit.Max,
                    limit.Severity)),
                cancellationToken);
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        return recordIds;
    }

    private async Task<RecordType> GetRecordTypeAsync(string code, CancellationToken cancellationToken)
    {
        var recordType = await recordTypeRepository.FindByCodeAsync(code, cancellationToken);

        if (recordType is null || !recordType.IsActive)
            throw new InvalidOperationException(
                $"Record type {code} is not available. Apply the database migrations.");

        return recordType;
    }

    private static decimal? ValueOf(InitialMeasurements measurements, string code)
    {
        return code switch
        {
            "HEART_RATE" => measurements.HeartRate,
            "OXYGEN_SATURATION" => measurements.OxygenSaturation,
            "GLUCOSE" => measurements.Glucose,
            "BLOOD_PRESSURE" => measurements.SystolicPressure,
            "BLOOD_PRESSURE_DIASTOLIC" => measurements.DiastolicPressure,
            _ => null
        };
    }
}
