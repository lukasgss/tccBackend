using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Localization.GetAllCitiesFromState;

public record GetAllCitiesFromStateQuery(int StateId) : IRequest<IList<DropdownDataResponse<string>>>;

public class GetAllCitiesFromStateQueryHandler
    : IRequestHandler<GetAllCitiesFromStateQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public GetAllCitiesFromStateQueryHandler(IAppDbContext dbContext, IMemoryCache memoryCache)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<DropdownDataResponse<string>>> Handle(GetAllCitiesFromStateQuery request,
        CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(CacheKeys.CitiesOfState(request.StateId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _dbContext.Cities
                .AsNoTracking()
                .Where(city => city.State.Id == request.StateId)
                .Select(city => new DropdownDataResponse<string>()
                {
                    Label = city.Name,
                    Value = city.Id.ToString()
                })
                .ToListAsync(cancellationToken);
        }) ?? [];
    }
}