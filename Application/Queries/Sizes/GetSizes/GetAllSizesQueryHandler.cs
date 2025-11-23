using System.Diagnostics.CodeAnalysis;
using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Ardalis.GuardClauses;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Sizes.GetSizes;

[ExcludeFromCodeCoverage]
public record GetAllSizesQuery : IRequest<IList<DropdownDataResponse<string>>>;

public class GetAllSizesQueryHandler : IRequestHandler<GetAllSizesQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IMemoryCache _memoryCache;

    public GetAllSizesQueryHandler(IMemoryCache memoryCache)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
    }

    public Task<IList<DropdownDataResponse<string>>> Handle(GetAllSizesQuery request,
        CancellationToken cancellationToken)
    {
        var sizes = _memoryCache.GetOrCreate(CacheKeys.Sizes, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            IList<DropdownDataResponse<string>> sizes = [];
            foreach (Size size in Enum.GetValues<Size>())
            {
                sizes.Add(new DropdownDataResponse<string>()
                {
                    Label = Enum.GetName(typeof(Size), size)!,
                    Value = ((int)size).ToString()
                });
            }

            return sizes;
        }) ?? [];

        return Task.FromResult(sizes);
    }
}