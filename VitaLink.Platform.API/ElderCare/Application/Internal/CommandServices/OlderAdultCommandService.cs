
using VitaLink.Platform.API.ElderCare.Application.Errors;
using VitaLink.Platform.API.ElderCare.Application.Services;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.Commands;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Application.Patterns;
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
}
