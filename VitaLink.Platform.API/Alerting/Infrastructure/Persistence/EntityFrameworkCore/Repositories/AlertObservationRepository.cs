using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Alerting.Domain.Model.Entities;
using VitaLink.Platform.API.Alerting.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Alerting.Domain.Repositories;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaLink.Platform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>Queries observations using the alert identifier.</summary>
public class AlertObservationRepository(AppDbContext context)
    : BaseRepository<AlertObservation, AlertObservationId>(context), IAlertObservationRepository
{
    public Task<AlertObservation?> FindByAlertAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        var id = new AlertId(alertId);
        return Context.Set<AlertObservation>().AsNoTracking()
            .SingleOrDefaultAsync(observation => observation.AlertId == id, cancellationToken);
    }
}
