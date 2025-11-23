using System.Diagnostics.CodeAnalysis;
using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Localization.GetAllStates;

[ExcludeFromCodeCoverage]
public record GetAllStatesQuery : IRequest<IList<DropdownDataResponse<string>>>;

public class GetAllStatesQueryHandler : IRequestHandler<GetAllStatesQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public GetAllStatesQueryHandler(IAppDbContext dbContext, IMemoryCache memoryCache)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<DropdownDataResponse<string>>> Handle(GetAllStatesQuery request,
        CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(CacheKeys.AllStates, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
            return await _dbContext.States
                .AsNoTracking()
                .Select(state => new DropdownDataResponse<string>
                {
                    Label = state.Name,
                    Value = state.Id.ToString()
                })
                .ToListAsync(cancellationToken);
        }) ?? [];
    }
}