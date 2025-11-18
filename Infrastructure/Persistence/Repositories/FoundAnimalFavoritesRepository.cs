using Application.Common.Interfaces.Entities.FoundAnimalFavoriteAlerts;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts.UserFavorites;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class FoundAnimalFavoritesRepository : GenericRepository<FoundAnimalFavorite>,
	IFoundAnimalFavoritesRepository
{
	private readonly AppDbContext _dbContext;

	public FoundAnimalFavoritesRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = Guard.Against.Null(dbContext);
	}

	public async Task<FoundAnimalFavorite?> GetFavoriteAlertAsync(Guid userId, Guid alertId)
	{
		return await _dbContext.FoundAnimalFavorites
			.SingleOrDefaultAsync(favorite => favorite.UserId == userId && favorite.FoundAnimalAlertId == alertId);
	}
}