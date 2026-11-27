using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the family caregiver links.
/// </summary>
public class FamilyCaregiverLinkRepository(AppDbContext context)
    : BaseRepository<FamilyCaregiverLink, Guid>(context),
        IFamilyCaregiverLinkRepository
{
}
