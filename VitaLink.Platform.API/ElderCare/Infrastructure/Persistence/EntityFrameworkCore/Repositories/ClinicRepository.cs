using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the Clinic aggregate.
/// </summary>
public class ClinicRepository(AppDbContext context)
    : BaseRepository<Clinic, ClinicId>(context),
        IClinicRepository
{
    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Clinic>()
            .AnyAsync(clinic => clinic.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Clinic>> ListOrderedByNameAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Clinic>()
            .OrderBy(clinic => clinic.Name)
            .ToListAsync(cancellationToken);
    }
}
