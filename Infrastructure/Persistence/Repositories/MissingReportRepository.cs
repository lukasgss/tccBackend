using Application.Common.Interfaces.Entities.AdoptionReports;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;

namespace Infrastructure.Persistence.Repositories;

public sealed class MissingReportRepository : GenericRepository<MissingAnimalReport>, IMissingReportRepository
{
	public MissingReportRepository(AppDbContext dbContext) : base(dbContext)
	{
	}
}