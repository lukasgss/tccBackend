using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AdoptionReportConfigurations : IEntityTypeConfiguration<AdoptionReport>
{
    public void Configure(EntityTypeBuilder<AdoptionReport> builder)
    {
        builder.ToTable("AdoptionReports");

        builder.Property(p => p.Reason)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.RejectedReason)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasOne(p => p.Owner)
            .WithMany(p => p.AdoptionReports);
    }
}