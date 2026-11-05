using Microsoft.Extensions.Options;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.IAM.Domain.Repositories;
using VitaLink.Platform.API.IAM.Domain.Services;
using VitaLink.Platform.API.Shared.Domain.Repositories;

namespace VitaLink.Platform.API.IAM.Infrastructure.Seeding;

/// <summary>
///     Creates one verified demo user per role, so the sign-in can be tested and presented
///     without a registration flow.
/// </summary>
/// <remarks>
///     It runs on startup only when <c>IAM:DemoUsers:Enabled</c> is <c>true</c> and a password
///     is configured. It is idempotent: users that already exist are skipped.
/// </remarks>
/// <param name="userRepository">The user repository.</param>
/// <param name="passwordHasher">The password hasher.</param>
/// <param name="unitOfWork">The unit of work.</param>
/// <param name="options">Demo user configuration.</param>
/// <param name="logger">The logger.</param>
public class DemoUserSeeder(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IOptions<DemoUserOptions> options,
    ILogger<DemoUserSeeder> logger)
{
    private static readonly (string Email, UserRole Role)[] DemoUsers =
    [
        ("older.adult@vitalink.demo", UserRole.OlderAdult),
        ("family.caregiver@vitalink.demo", UserRole.FamilyCaregiver),
        ("care.provider@vitalink.demo", UserRole.CareProvider)
    ];

    /// <summary>
    ///     Creates the demo users that do not exist yet.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (!settings.Enabled) return;

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            logger.LogWarning("Demo users are enabled but {Section}:Password is not configured; skipping.",
                DemoUserOptions.SectionName);
            return;
        }

        var created = 0;
        foreach (var (address, role) in DemoUsers)
        {
            var email = new Email(address);
            if (await userRepository.ExistsByEmailAsync(email, cancellationToken)) continue;

            var user = new User(email, passwordHasher.Hash(settings.Password), role);
            user.VerifyAccount();
            await userRepository.AddAsync(user, cancellationToken);
            created++;
        }

        if (created == 0) return;

        await unitOfWork.CompleteAsync(cancellationToken);
        logger.LogInformation("Created {Count} demo user(s).", created);
    }
}
