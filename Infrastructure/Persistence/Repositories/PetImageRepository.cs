using Application.Common.Interfaces.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PetImageRepository : GenericRepository<PetImage>, IPetImageRepository
{
	private readonly AppDbContext _dbContext;

	public PetImageRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<List<PetImage>> GetImagesFromPetByIdAsync(Guid petId)
	{
		return await _dbContext.PetImage
			.AsNoTracking()
			.Where(image => image.PetId == petId)
			.ToListAsync();
	}
}