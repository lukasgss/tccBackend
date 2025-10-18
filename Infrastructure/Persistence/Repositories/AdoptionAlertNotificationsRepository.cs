using Application.Common.Interfaces.Entities.AdoptionAlertNotifications;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts.Notifications;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionAlertNotificationsRepository : GenericRepository<AdoptionAlertNotification>,
    IAdoptionAlertNotificationsRepository
{
    private readonly AppDbContext _dbContext;

    public AdoptionAlertNotificationsRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<AdoptionAlertNotification?> GetByIdAsync(long notificationId)
    {
        return await _dbContext
            .AdoptionAlertNotifications
            .Include(notification => notification.Users)
            .SingleOrDefaultAsync(notification => notification.Id == notificationId);
    }

    public async Task ReadAllAsync(Guid userId)
    {
        await _dbContext.AdoptionAlertNotifications
            .Where(notification => notification.Users.Any(user => user.Id == userId) &&
                                   !notification.HasBeenRead)
            .ExecuteUpdateAsync(notification =>
                notification.SetProperty(n => n.HasBeenRead, true));
    }
}