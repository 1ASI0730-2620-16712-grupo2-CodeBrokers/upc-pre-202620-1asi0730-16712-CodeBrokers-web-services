using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VitaLink.Platform.API.PreventiveMonitoring.Domain.Model.Entities;

namespace VitaLink.Platform.API.PreventiveMonitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class RecordTypeConfiguration : IEntityTypeConfiguration<RecordType>
{
    public void Configure(EntityTypeBuilder<RecordType> builder)
    {
        builder.ToTable("record_types", "monitoring", table =>
        {
            table.HasCheckConstraint("ck_record_types_range", "min_valid_value IS NULL OR max_valid_value IS NULL OR min_valid_value < max_valid_value");
            table.HasCheckConstraint("ck_record_types_unit", "requires_value = FALSE OR unit IS NOT NULL");
        });

        builder.HasKey(t => t.Code);
        builder.Property(t => t.Code).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Unit).HasMaxLength(20);
        builder.Property(t => t.RequiresValue).IsRequired();
        builder.Property(t => t.MinValidValue).HasPrecision(18, 2);
        builder.Property(t => t.MaxValidValue).HasPrecision(18, 2);
    }
}