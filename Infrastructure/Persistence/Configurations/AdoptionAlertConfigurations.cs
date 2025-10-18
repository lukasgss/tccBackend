using Domain.Entities.Alerts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AdoptionAlertConfigurations : IEntityTypeConfiguration<AdoptionAlert>
{
    public void Configure(EntityTypeBuilder<AdoptionAlert> builder)
    {
        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.RegistrationDate)
            .IsRequired();

        builder.OwnsOne(p => p.AdoptionForm, owned =>
        {
            owned.Property(p => p.FileName)
                .IsRequired()
                .HasMaxLength(256);

            owned.Property(p => p.FileUrl)
                .IsRequired()
                .HasMaxLength(256);
        });

        builder.HasOne(p => p.Pet);

        builder.HasOne(p => p.User)
            .WithMany(p => p.AdoptionAlerts);

        builder.HasOne(p => p.State)
            .WithMany(p => p.AdoptionAlerts);

        builder.HasOne(p => p.City)
            .WithMany(p => p.AdoptionAlerts);

        builder.Property(p => p.Neighborhood)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(p => p.AdoptionFavorites)
            .WithOne(p => p.AdoptionAlert);

        builder.HasMany(p => p.AlertNotifications)
            .WithOne(p => p.AdoptionAlert);
    }
}