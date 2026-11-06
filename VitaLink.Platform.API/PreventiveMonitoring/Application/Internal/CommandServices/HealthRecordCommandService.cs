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
        // 1. Validar referencias requeridas
        if (command.OlderAdultId == Guid.Empty || command.RecordedBy == Guid.Empty)
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidReference);
        }

        // 2. Validar y normalizar código del tipo de registro
        if (string.IsNullOrWhiteSpace(command.RecordTypeCode))
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidRecordType);
        }

        var normalizedCode = command.RecordTypeCode.Trim().ToUpperInvariant();

        // 3. Buscar tipo y validar que exista y esté activo
        var recordType = await recordTypeRepository.FindByCodeAsync(normalizedCode, cancellationToken);
        if (recordType is null || !recordType.IsActive)
        {
            return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidRecordType);
        }

        // 4. Validar la nota si está presente
        string? normalizedNote = command.Note;
        if (command.Note is not null)
        {
            if (string.IsNullOrWhiteSpace(command.Note))
            {
                return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidNote);
            }
            normalizedNote = command.Note.Trim();
        }

        // 5. Validar obligatoriedad del valor / registro vacío
        if (!command.MeasurementValue.HasValue)
        {
            if (recordType.RequiresValue)
            {
                return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.ValueRequired);
            }

            if (normalizedNote is null)
            {
                return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.EmptyRecord);
            }
        }
        else
        {
            // 6. Validar rango del valor numérico
            var val = command.MeasurementValue.Value;
            if (val < 0m || val > 99999999.99m)
            {
                return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.ValueOutOfRange);
            }

            if (recordType.MinValidValue.HasValue && val < recordType.MinValidValue.Value)
            {
                return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.ValueOutOfRange);
            }

            if (recordType.MaxValidValue.HasValue && val > recordType.MaxValidValue.Value)
            {
                return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.ValueOutOfRange);
            }

            // 7. Validar unidad de medida si el catálogo la especifica
            if (!string.IsNullOrWhiteSpace(recordType.Unit))
            {
                if (string.IsNullOrWhiteSpace(command.Unit) ||
                    !string.Equals(command.Unit.Trim(), recordType.Unit.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return new Result<HealthRecord, CreateHealthRecordError>.Failure(CreateHealthRecordError.InvalidUnit);
                }
            }
        }

        var updatedCommand = command with
        {
            RecordTypeCode = normalizedCode,
            Note = normalizedNote
        };

        try
        {
            var healthRecord = new HealthRecord(updatedCommand);
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