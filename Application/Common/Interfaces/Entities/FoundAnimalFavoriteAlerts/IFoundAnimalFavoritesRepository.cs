using Application.Common.Interfaces.GenericRepository;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Common.Interfaces.Entities.FoundAnimalFavoriteAlerts;

public interface IFoundAnimalFavoritesRepository : IGenericRepository<FoundAnimalFavorite>
{
	Task<FoundAnimalFavorite?> GetFavoriteAlertAsync(Guid userId, Guid alertId);
}