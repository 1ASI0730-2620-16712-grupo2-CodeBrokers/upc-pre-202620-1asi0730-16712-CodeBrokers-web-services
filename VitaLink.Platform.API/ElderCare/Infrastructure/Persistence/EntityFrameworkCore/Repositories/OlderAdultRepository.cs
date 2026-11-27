using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Model.ValueObjects;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

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

    public async Task<IReadOnlyList<OlderAdult>> SearchAsync(
        string? search,
        Guid? careProviderId,
        bool? isMonitoringActive,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<OlderAdult>().AsQueryable();

        if (careProviderId is not null)
            query = query.Where(olderAdult =>
                olderAdult.AssignedProviderId == careProviderId);

        if (isMonitoringActive is not null)
            query = query.Where(olderAdult =>
                olderAdult.IsMonitoringActive == isMonitoringActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{EscapeLikePattern(search.Trim())}%";

            query = query.Where(olderAdult =>
                EF.Functions.ILike(
                    olderAdult.FullName.FirstName + " " + olderAdult.FullName.LastName,
                    pattern,
                    "\\"));
        }

        return await query
            .OrderBy(olderAdult => olderAdult.FullName.LastName)
            .ThenBy(olderAdult => olderAdult.FullName.FirstName)
            .ToListAsync(cancellationToken);
    }

    // Escapes the characters that have a special meaning in a LIKE pattern.
    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}
