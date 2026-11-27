using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the simulated registry of professional licenses.
/// </summary>
public class RegisteredLicenseRepository(AppDbContext context)
    : BaseRepository<RegisteredLicense, Guid>(context),
        IRegisteredLicenseRepository
{
    public async Task<bool> ExistsByNumberAsync(
        string licenseNumber,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<RegisteredLicense>()
            .AnyAsync(
                license => license.LicenseNumber == licenseNumber,
                cancellationToken);
    }
}
