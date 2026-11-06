using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VitaLink.Platform.API.PreventiveMonitoring.Application.Errors;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Aggregates;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Commands;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Repositories;
using VitaLink.Platform.API.Shared.Application.Patterns;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.PreventiveMonitoring.Application.Internal.CommandServices;

public class HealthRecordCommandService(
    IHealthRecordRepository healthRecordRepository,
    IRecordTypeRepository recordTypeRepository,
    IUnitOfWork unitOfWork) : IHealthRecordCommandService
{
    public async Task<Result<HealthRecord, CreateHealthRecordError>> Handle(
        CreateHealthRecordCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OlderAdultId == Guid.Empty || command.RecordedBy == Guid.Empty)
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidReference);
        }

        if (string.IsNullOrWhiteSpace(command.RecordTypeCode))
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidRecordType);
        }

        var recordType = await recordTypeRepository.FindByCodeAsync(command.RecordTypeCode, cancellationToken);
        if (recordType is null)
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidRecordType);
        }

        try
        {
            var healthRecord = new HealthRecord(command);
            await healthRecordRepository.AddAsync(healthRecord, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<HealthRecord, CreateHealthRecordError>.Success(healthRecord);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidRecordType);
        }
    }
}