using Application.Common.Interfaces.GenericRepository;
using Application.Events.AdoptionAlerts.CreatedAdoptionAlert;
using Domain.Entities.Alerts.UserPreferences;
using Domain.Events;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences;

public interface IAdoptionUserPreferencesRepository : IGenericRepository<AdoptionUserPreferences>
{
    Task<AdoptionUserPreferences?> GetUserPreferences(Guid userId);

    Task<List<UserThatMatchPreferences>> GetUsersThatMatchPreferences(AdoptionAlertCreated adoptionAlert,
        double matchThreshold = 0.8);
}