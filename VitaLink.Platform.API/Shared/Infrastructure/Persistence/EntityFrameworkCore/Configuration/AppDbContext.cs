using Microsoft.EntityFrameworkCore;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context shared by all bounded contexts.
/// </summary>
/// <remarks>
///     Each bounded context maps its entities with its own
///     <see cref="IEntityTypeConfiguration{TEntity}"/> classes, placed in its
///     <c>Infrastructure/Persistence/EntityFrameworkCore/Configuration</c> folder and using its own
///     PostgreSQL schema (e.g. <c>alerting</c>, <c>elder_care</c>). They are discovered automatically,
///     so this class does not need to change when a new entity is added.
/// </remarks>
/// <param name="options">The options for this context.</param>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply the Fluent API configurations defined by each bounded context
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.UseSnakeCaseNamingConvention();
    }
}
