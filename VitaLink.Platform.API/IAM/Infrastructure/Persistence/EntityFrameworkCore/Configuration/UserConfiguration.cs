using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.IAM.Domain.Model.Aggregates;
using VitaLink.Platform.API.IAM.Domain.Model.ValueObjects;
using VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace VitaLink.Platform.API.IAM.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Fluent API mapping of the <see cref="User"/> aggregate to the <c>iam.users</c> table.
/// </summary>
/// <remarks>
///     Discovered automatically by the shared <c>AppDbContext</c>. Roles are stored in upper
///     snake case (<c>CARE_PROVIDER</c>) and the check constraints follow <c>schema.sql</c>.
/// </remarks>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "iam", table =>
        {
            table.HasCheckConstraint("ck_users_role",
                "role IN ('OLDER_ADULT','FAMILY_CAREGIVER','CARE_PROVIDER')");
            table.HasCheckConstraint("ck_users_email_format",
                "email ~* '^[^@[:space:]]+@[^@[:space:]]+\\.[^@[:space:]]+$'");
            table.HasCheckConstraint("ck_users_last_access",
                "last_access_at IS NULL OR last_access_at >= created_at");
        });

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => new UserId(value))
            .ValueGeneratedNever();

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => new Email(value))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasConversion(hash => hash.Value, value => new PasswordHash(value))
            .HasMaxLength(PasswordHash.MaxLength)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion(
                role => role.ToString().ToSnakeCase().ToUpperInvariant(),
                value => Enum.Parse<UserRole>(value.Replace("_", string.Empty), true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.IsVerified).IsRequired();
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.LastAccessAt);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("uq_users_email");
    }
}
