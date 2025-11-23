using System.Diagnostics.CodeAnalysis;
using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Species.GetSpecies;

[ExcludeFromCodeCoverage]
public record GetAllSpeciesQuery : IRequest<IList<DropdownDataResponse<string>>>;

public class GetAllSpeciesQueryHandler : IRequestHandler<GetAllSpeciesQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public GetAllSpeciesQueryHandler(IAppDbContext dbContext, IMemoryCache memoryCache)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<DropdownDataResponse<string>>> Handle(GetAllSpeciesQuery request,
        CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(CacheKeys.Species, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            return await _dbContext.Species
                .AsNoTracking()
                .Select(species => new DropdownDataResponse<string>()
                {
                    Label = species.Name,
                    Value = species.Id.ToString()
                })
                .ToListAsync(cancellationToken);
        }) ?? [];
    }
}