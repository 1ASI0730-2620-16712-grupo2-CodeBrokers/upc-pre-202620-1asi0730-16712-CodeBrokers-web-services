using Microsoft.EntityFrameworkCore;
using VitaLink.ElderCare.Domain.Model.Aggregates;
using VitaLink.ElderCare.Domain.Model.ValueObjects;
using VitaLink.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the OlderAdult aggregate.
/// </summary>
public class OlderAdultRepository(AppDbContext context)
    : BaseRepository<OlderAdult, OlderAdultId>(context),
        IOlderAdultRepository
{
    public async Task<IReadOnlyList<OlderAdult>> FindByCareProviderIdAsync(
        CareProviderId providerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(providerId);

        return await Context.Set<OlderAdult>()
            .Where(olderAdult =>
                olderAdult.AssignedProviderId == providerId.Value)
            .ToListAsync(cancellationToken);
    }
}