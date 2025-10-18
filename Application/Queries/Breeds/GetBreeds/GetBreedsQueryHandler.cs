using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Breeds.GetBreeds;

public record GetBreedsQuery(int SpeciesId) : IRequest<IList<DropdownDataResponse<string>>>;

public class GetBreedsQueryHandler : IRequestHandler<GetBreedsQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public GetBreedsQueryHandler(IAppDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = Guard.Against.Null(dbContext);
        _memoryCache = Guard.Against.Null(memoryCache);
    }

    public async Task<IList<DropdownDataResponse<string>>> Handle(GetBreedsQuery request,
        CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(CacheKeys.BreedOfSpecies(request.SpeciesId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            return await _dbContext.Breeds
                .AsNoTracking()
                .Where(breed => breed.SpeciesId == request.SpeciesId)
                .Select(breed => new DropdownDataResponse<string>()
                {
                    Label = breed.Name,
                    Value = breed.Id.ToString()
                })
                .OrderBy(breed => breed.Label)
                .ToListAsync(cancellationToken);
        }) ?? [];
    }
}