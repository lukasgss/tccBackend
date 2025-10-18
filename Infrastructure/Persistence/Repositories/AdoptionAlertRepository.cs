using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionAlertRepository : GenericRepository<AdoptionAlert>, IAdoptionAlertRepository
{
    private readonly AppDbContext _dbContext;

    public AdoptionAlertRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<AdoptionAlert?> GetByIdAsync(Guid alertId)
    {
        return await _dbContext.AdoptionAlerts
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Images)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Colors)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Species)
            .Include(alert => alert.User)
            .Include(alert => alert.State)
            .Include(alert => alert.City)
            .SingleOrDefaultAsync(alert => alert.Id == alertId);
    }
}