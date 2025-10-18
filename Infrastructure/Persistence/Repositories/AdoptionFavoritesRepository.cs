using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts.UserFavorites;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionFavoritesRepository : GenericRepository<AdoptionFavorite>, IAdoptionFavoritesRepository
{
    private readonly AppDbContext _dbContext;

    public AdoptionFavoritesRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<AdoptionFavorite?> GetFavoriteAlertAsync(Guid userId, Guid alertId)
    {
        return await _dbContext.AdoptionFavorites
            .SingleOrDefaultAsync(favorite => favorite.UserId == userId && favorite.AdoptionAlertId == alertId);
    }
}