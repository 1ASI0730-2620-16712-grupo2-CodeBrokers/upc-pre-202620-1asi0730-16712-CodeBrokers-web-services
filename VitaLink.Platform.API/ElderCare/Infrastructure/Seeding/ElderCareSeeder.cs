using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;
using VitaLink.Platform.API.ElderCare.Domain.Repositories;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Seeding;

/// <summary>
///     Creates the test data of ElderCare: three clinics and ten registered professional licenses.
/// </summary>
/// <remarks>
///     It runs on startup, after the migrations, and is idempotent: rows that already exist are skipped.
///     The licenses are fictitious; they stand in for a real professional registry.
/// </remarks>
public class ElderCareSeeder(
    IClinicRepository clinicRepository,
    IRegisteredLicenseRepository registeredLicenseRepository,
    IUnitOfWork unitOfWork,
    ILogger<ElderCareSeeder> logger)
{
    private static readonly (string Name, string District)[] Clinics =
    [
        ("Clínica Vida Plena", "Miraflores"),
        ("Centro Médico Los Olivos", "Los Olivos"),
        ("Clínica Santa Aurora", "San Isidro")
    ];

    private static readonly (string Number, string Holder)[] Licenses =
    [
        ("10234", "Test license holder 1"),
        ("20345", "Test license holder 2"),
        ("30456", "Test license holder 3"),
        ("40567", "Test license holder 4"),
        ("50678", "Test license holder 5"),
        ("60789", "Test license holder 6"),
        ("70890", "Test license holder 7"),
        ("80901", "Test license holder 8"),
        ("91012", "Test license holder 9"),
        ("1234567", "Test license holder 10")
    ];

    /// <summary>
    ///     Creates the clinics and licenses that do not exist yet.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var added = 0;

        foreach (var (name, district) in Clinics)
        {
            if (await clinicRepository.ExistsByNameAsync(name, cancellationToken)) continue;

            await clinicRepository.AddAsync(new Clinic(name, district), cancellationToken);
            added++;
        }

        foreach (var (number, holder) in Licenses)
        {
            if (await registeredLicenseRepository.ExistsByNumberAsync(number, cancellationToken)) continue;

            await registeredLicenseRepository.AddAsync(new RegisteredLicense(number, holder), cancellationToken);
            added++;
        }

        if (added == 0) return;

        await unitOfWork.CompleteAsync(cancellationToken);
        logger.LogInformation("Created {Count} ElderCare test row(s).", added);
    }
}
