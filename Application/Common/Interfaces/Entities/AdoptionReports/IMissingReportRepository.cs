using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.AdoptionReports;

public interface IMissingReportRepository : IGenericRepository<MissingAnimalReport>
{
}