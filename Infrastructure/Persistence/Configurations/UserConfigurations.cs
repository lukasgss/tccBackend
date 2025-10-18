using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(p => p.Image)
            .IsRequired()
            .HasMaxLength(180);

        builder.Property(p => p.ReceivesOnlyWhatsAppMessages)
            .IsRequired();

        builder.Property(p => p.DefaultAdoptionFormUrl)
            .HasMaxLength(256);

        builder.Property(p => p.PasswordHash)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.SecurityStamp)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(p => p.ConcurrencyStamp)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasMany(p => p.AdoptionAlerts)
            .WithOne(p => p.User);

        builder.HasOne(p => p.FoundAnimalUserPreferences)
            .WithOne(p => p.User);

        builder.HasOne(p => p.AdoptionUserPreferences)
            .WithOne(p => p.User);

        builder.HasMany(p => p.FoundAnimalAlertNotifications)
            .WithMany(p => p.Users);

        builder.HasMany(p => p.AdoptionFavorites)
            .WithOne(p => p.User);

        builder.HasMany(p => p.AdoptionReports)
            .WithOne(p => p.Owner);

        builder.HasMany(p => p.AdoptionAlertNotifications)
            .WithMany(p => p.Users);
    }
}