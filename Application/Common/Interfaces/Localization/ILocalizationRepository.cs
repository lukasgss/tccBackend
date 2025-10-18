using Domain.Entities;

namespace Application.Common.Interfaces.Localization;

public interface ILocalizationRepository
{
	Task<State?> GetStateById(int stateId);
	Task<List<State>> GetAllStates();
	Task<List<City>> GetCitiesFromState(int stateId);
	Task<City?> GetCityById(int cityId);
}