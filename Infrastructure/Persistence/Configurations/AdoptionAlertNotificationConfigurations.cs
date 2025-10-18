using Domain.Entities.Alerts.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AdoptionAlertNotificationConfigurations : IEntityTypeConfiguration<AdoptionAlertNotification>
{
    public void Configure(EntityTypeBuilder<AdoptionAlertNotification> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(p => p.TimeStampUtc)
            .IsRequired();

        builder.HasMany(p => p.Users)
            .WithMany(p => p.AdoptionAlertNotifications);

        builder.HasOne(p => p.AdoptionAlert)
            .WithMany(p => p.AlertNotifications);
    }
}