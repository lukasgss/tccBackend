using Application.Common.Interfaces.GenericRepository;
using Domain.ValueObjects;

namespace Application.Common.Interfaces.Entities;

public interface IPetImageRepository : IGenericRepository<PetImage>
{
	Task<List<PetImage>> GetImagesFromPetByIdAsync(Guid petId);
}