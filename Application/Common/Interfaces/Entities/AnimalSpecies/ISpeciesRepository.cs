using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.AnimalSpecies;

public interface ISpeciesRepository : IGenericRepository<Species>
{
	Task<Species?> GetSpeciesByIdAsync(int speciesId);
	Task<List<Species>> GetMultipleSpeciesByIdAsync(IEnumerable<int> speciesIds);
}