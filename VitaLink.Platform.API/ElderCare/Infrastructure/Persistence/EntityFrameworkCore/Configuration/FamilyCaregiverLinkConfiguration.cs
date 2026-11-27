using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.ElderCare.Domain.Model.Aggregates;

namespace VitaLink.Platform.API.ElderCare.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     EF Core mapping for the family caregiver links.
/// </summary>
public class FamilyCaregiverLinkConfiguration : IEntityTypeConfiguration<FamilyCaregiverLink>
{
    public void Configure(EntityTypeBuilder<FamilyCaregiverLink> builder)
    {
        builder.ToTable("family_caregiver_links", "elder_care");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(l => l.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(l => l.OlderAdultId)
            .HasColumnName("older_adult_id")
            .IsRequired();

        builder.Property(l => l.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // A caregiver is linked once to each older adult.
        builder.HasIndex(l => new { l.UserId, l.OlderAdultId })
            .IsUnique();

        builder.HasIndex(l => l.OlderAdultId);
    }
}
