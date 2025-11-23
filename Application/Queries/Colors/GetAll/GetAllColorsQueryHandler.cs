using System.Diagnostics.CodeAnalysis;
using Application.Common.Cache;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Colors.GetAll;

[ExcludeFromCodeCoverage]
public record GetAllColorsQuery : IRequest<IList<ColorResponse>>;

public class GetAllColorsQueryHandler : IRequestHandler<GetAllColorsQuery, IList<ColorResponse>>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IAppDbContext _dbContext;

    public GetAllColorsQueryHandler(IMemoryCache memoryCache, IAppDbContext dbContext)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<ColorResponse>> Handle(GetAllColorsQuery request,
        CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(CacheKeys.Colors, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            return await _dbContext.Colors
                .AsNoTracking()
                .Select(color => new ColorResponse(
                        color.Id,
                        color.Name,
                        color.HexCode
                    )
                )
                .ToListAsync(cancellationToken);
        }) ?? [];
    }
}