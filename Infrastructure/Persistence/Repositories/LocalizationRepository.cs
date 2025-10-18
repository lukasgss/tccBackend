using Application.Common.Interfaces.Localization;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LocalizationRepository : ILocalizationRepository
{
	private readonly AppDbContext _dbContext;

	public LocalizationRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<State?> GetStateById(int stateId)
	{
		return await _dbContext.States
			.SingleOrDefaultAsync(state => state.Id == stateId);
	}

	public async Task<List<State>> GetAllStates()
	{
		return await _dbContext.States
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<List<City>> GetCitiesFromState(int stateId)
	{
		return await _dbContext.Cities
			.AsNoTracking()
			.Include(city => city.State)
			.Where(city => city.State.Id == stateId)
			.ToListAsync();
	}

	public async Task<City?> GetCityById(int cityId)
	{
		return await _dbContext.Cities
			.SingleOrDefaultAsync(city => city.Id == cityId);
	}
}