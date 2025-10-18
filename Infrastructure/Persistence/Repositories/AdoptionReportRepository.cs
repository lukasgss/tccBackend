using Application.Common.Interfaces.Entities.AdoptionReports;
using Ardalis.GuardClauses;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionReportRepository : GenericRepository<AdoptionReport>, IAdoptionReportRepository
{
    private readonly AppDbContext _dbContext;

    public AdoptionReportRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }
}