using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Domain.Repositories;

public interface IFamilyCaregiverLinkRepository
    : IBaseRepository<FamilyCaregiverLink, Guid>
{
}
