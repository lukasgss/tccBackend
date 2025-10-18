using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts.Notifications;

namespace Application.Common.Interfaces.Entities.AdoptionAlertNotifications;

public interface IAdoptionAlertNotificationsRepository : IGenericRepository<AdoptionAlertNotification>
{
    Task<AdoptionAlertNotification?> GetByIdAsync(long notificationId);
    Task ReadAllAsync(Guid userId);
}