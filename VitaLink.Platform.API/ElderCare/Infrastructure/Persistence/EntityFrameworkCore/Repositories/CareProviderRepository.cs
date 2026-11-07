using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the CareProvider aggregate.
/// </summary>
public class CareProviderRepository(AppDbContext context)
    : BaseRepository<CareProvider, CareProviderId>(context),
        ICareProviderRepository
{
    public async Task<CareProvider?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<CareProvider>()
            .FirstOrDefaultAsync(
                provider => provider.UserId == userId,
                cancellationToken);
    }
}