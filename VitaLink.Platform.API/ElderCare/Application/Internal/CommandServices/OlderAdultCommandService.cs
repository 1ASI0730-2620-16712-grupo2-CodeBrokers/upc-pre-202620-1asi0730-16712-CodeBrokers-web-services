
using VitaLink.Platform.API.ElderCare.Domain.Model.Errors;
using VitaLink.Platform.API.ElderCare.Application.CommandServices;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Commands;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.ElderCare.Domain.Services;
using VitaLink.Platform.API.Shared.Application.Model;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Application.Internal.CommandServices;

/// <summary>
///     Handles commands that modify older adult aggregates.
/// </summary>
public class OlderAdultCommandService(
    IOlderAdultRepository olderAdultRepository,
    ICareProviderRepository careProviderRepository,
    IUnitOfWork unitOfWork) : IOlderAdultCommandService
{
    public async Task<Result<OlderAdult, LinkCareProviderError>> Handle(
        LinkCareProviderCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OlderAdultId == Guid.Empty ||
            command.CareProviderId == Guid.Empty)
        {
            return new Result<OlderAdult, LinkCareProviderError>.Failure(
                LinkCareProviderError.UnexpectedError);
        }

        var olderAdult = await olderAdultRepository.FindByIdAsync(
            new OlderAdultId(command.OlderAdultId),
            cancellationToken);

        if (olderAdult is null)
        {
            return new Result<OlderAdult, LinkCareProviderError>.Failure(
                LinkCareProviderError.OlderAdultNotFound);
        }

        var careProvider = await careProviderRepository.FindByIdAsync(
            new CareProviderId(command.CareProviderId),
            cancellationToken);

        if (careProvider is null)
        {
            return new Result<OlderAdult, LinkCareProviderError>.Failure(
                LinkCareProviderError.CareProviderNotFound);
        }

        if (!careProvider.IsActive)
        {
            return new Result<OlderAdult, LinkCareProviderError>.Failure(
                LinkCareProviderError.CareProviderInactive);
        }

        // Repeating the same association is idempotent.
        if (olderAdult.AssignedProviderId == careProvider.Id.Value)
        {
            return new Result<OlderAdult, LinkCareProviderError>.Success(
                olderAdult);
        }

        // An older adult cannot have two providers simultaneously.
        if (!olderAdult.IsUnlinked())
        {
            return new Result<OlderAdult, LinkCareProviderError>.Failure(
                LinkCareProviderError.AlreadyAssigned);
        }

        olderAdult.LinkCareProvider(careProvider.Id.Value);

        olderAdultRepository.Update(olderAdult);

        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<OlderAdult, LinkCareProviderError>.Success(
            olderAdult);
    }

    public async Task<Result<OlderAdult, SaveOlderAdultError>> Handle(
        CreateOlderAdultCommand command,
        CancellationToken cancellationToken = default)
    {
        var errors = Validate(
            command.FirstName,
            command.LastName,
            command.BirthDate,
            command.Street,
            command.District,
            command.City,
            command.EmergencyContactName,
            command.EmergencyContactPhone,
            command.IsMonitoringActive);

        if (errors.Count > 0)
            return Fail(new SaveOlderAdultError.InvalidFields(errors));

        if (command.CareProviderId is not null)
        {
            var careProvider = await careProviderRepository.FindByIdAsync(
                new CareProviderId(command.CareProviderId.Value),
                cancellationToken);

            if (careProvider is null)
                return Fail(new SaveOlderAdultError.CareProviderNotFound());

            if (!careProvider.IsActive)
                return Fail(new SaveOlderAdultError.CareProviderInactive());
        }

        var olderAdult = new OlderAdult(
            new FullName(command.FirstName!, command.LastName!),
            command.BirthDate!.Value,
            BuildAddress(command.Street, command.District, command.City));

        olderAdult.UpdateProfile(
            olderAdult.FullName,
            olderAdult.BirthDate,
            olderAdult.Address,
            BuildEmergencyContact(
                command.EmergencyContactName,
                command.EmergencyContactPhone),
            command.IsMonitoringActive);

        if (command.CareProviderId is not null)
            olderAdult.LinkCareProvider(command.CareProviderId.Value);

        await olderAdultRepository.AddAsync(olderAdult, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<OlderAdult, SaveOlderAdultError>.Success(olderAdult);
    }

    public async Task<Result<OlderAdult, SaveOlderAdultError>> Handle(
        UpdateOlderAdultCommand command,
        CancellationToken cancellationToken = default)
    {
        var olderAdult = await olderAdultRepository.FindByIdAsync(
            new OlderAdultId(command.OlderAdultId),
            cancellationToken);

        if (olderAdult is null)
            return Fail(new SaveOlderAdultError.OlderAdultNotFound());

        // An invalid update is rejected as a whole: the stored version stays as it was.
        var errors = Validate(
            command.FirstName,
            command.LastName,
            command.BirthDate,
            command.Street,
            command.District,
            command.City,
            command.EmergencyContactName,
            command.EmergencyContactPhone,
            command.IsMonitoringActive);

        if (errors.Count > 0)
            return Fail(new SaveOlderAdultError.InvalidFields(errors));

        olderAdult.UpdateProfile(
            new FullName(command.FirstName!, command.LastName!),
            command.BirthDate!.Value,
            BuildAddress(command.Street, command.District, command.City),
            BuildEmergencyContact(
                command.EmergencyContactName,
                command.EmergencyContactPhone),
            command.IsMonitoringActive);

        // The aggregate was loaded by the repository, so the context already tracks it. Calling Update here
        // would try to track the replaced address and emergency contact as new instances; change detection
        // handles the replacement.
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<OlderAdult, SaveOlderAdultError>.Success(olderAdult);
    }

    private static IReadOnlyList<FieldError> Validate(
        string? firstName,
        string? lastName,
        DateOnly? birthDate,
        string? street,
        string? district,
        string? city,
        string? emergencyContactName,
        string? emergencyContactPhone,
        bool isMonitoringActive)
    {
        return OlderAdultProfileRules.Validate(
            firstName,
            lastName,
            birthDate,
            street,
            district,
            city,
            emergencyContactName,
            emergencyContactPhone,
            isMonitoringActive,
            DateOnly.FromDateTime(DateTime.UtcNow));
    }

    private static Address? BuildAddress(
        string? street,
        string? district,
        string? city)
    {
        if (string.IsNullOrWhiteSpace(street) &&
            string.IsNullOrWhiteSpace(district) &&
            string.IsNullOrWhiteSpace(city))
            return null;

        return new Address(street, district, city);
    }

    private static EmergencyContact? BuildEmergencyContact(
        string? name,
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(name) &&
            string.IsNullOrWhiteSpace(phone))
            return null;

        return new EmergencyContact(name, phone);
    }

    private static Result<OlderAdult, SaveOlderAdultError> Fail(
        SaveOlderAdultError error)
    {
        return new Result<OlderAdult, SaveOlderAdultError>.Failure(error);
    }
}
