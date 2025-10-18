using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;

public interface IAdoptionFavoritesRepository : IGenericRepository<AdoptionFavorite>
{
    Task<AdoptionFavorite?> GetFavoriteAlertAsync(Guid userId, Guid alertId);
}