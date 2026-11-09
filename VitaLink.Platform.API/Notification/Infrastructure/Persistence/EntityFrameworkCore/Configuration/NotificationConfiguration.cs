using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.Notification.Domain.Model.ValueObjects;
using NotificationAggregate = VitaLink.Platform.API.Notification.Domain.Model.Aggregates.Notification;

namespace VitaLink.Platform.API.Notification.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Fluent API mapping of the Notification aggregate to the <c>notification.notifications</c> table.
/// </summary>
/// <remarks>
///     Discovered automatically by the shared <c>AppDbContext</c>. Column names follow the
///     snake case convention of the solution, and enum values are stored in upper case
///     to match the check constraints of the database schema.
/// </remarks>
public class NotificationConfiguration : IEntityTypeConfiguration<NotificationAggregate>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<NotificationAggregate> builder)
    {
        builder.ToTable("notifications", "notification", table =>
        {
            table.HasCheckConstraint("ck_n_channel", "channel IN ('EMAIL','SMS')");
            table.HasCheckConstraint("ck_n_status", "status IN ('PENDING','SENT','FAILED')");
            table.HasCheckConstraint("ck_n_attempts", "attempts BETWEEN 0 AND 5");
        });

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id)
            .HasConversion(id => id.Value, value => new NotificationId(value))
            .ValueGeneratedNever();

        builder.Property(n => n.RecipientId).IsRequired();
        builder.Property(n => n.RelatedAlertId);

        builder.Property(n => n.Channel)
            .HasConversion(
                channel => channel.ToString().ToUpperInvariant(),
                value => Enum.Parse<NotificationChannel>(value, true))
            .HasMaxLength(10)
            .IsRequired();

        builder.OwnsOne(n => n.Content, content =>
        {
            content.WithOwner().HasForeignKey("Id");
            content.Property(c => c.Subject)
                .HasColumnName("subject")
                .HasMaxLength(MessageContent.MaxSubjectLength)
                .IsRequired();
            content.Property(c => c.Body)
                .HasColumnName("body")
                .IsRequired();
        });
        builder.Navigation(n => n.Content).IsRequired();

        builder.Property(n => n.Status)
            .HasConversion(
                status => status.ToString().ToUpperInvariant(),
                value => Enum.Parse<DeliveryStatus>(value, true))
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(n => n.Attempts)
            .HasConversion<short>()
            .IsRequired();

        builder.Property(n => n.ProviderMessageId).HasMaxLength(120);
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.Property(n => n.SentAt);
        builder.Property(n => n.FailureReason).HasMaxLength(NotificationAggregate.MaxFailureReasonLength);

        builder.HasIndex(n => new { n.RecipientId, n.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("ix_n_recipient_created");

        builder.HasIndex(n => new { n.Status, n.Attempts })
            .HasFilter("status = 'FAILED'")
            .HasDatabaseName("ix_n_retryable");

        builder.HasIndex(n => new { n.RelatedAlertId, n.RecipientId, n.Channel })
            .IsUnique()
            .HasDatabaseName("ux_n_alert_recipient_channel");
    }
}