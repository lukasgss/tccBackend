using System.Diagnostics.CodeAnalysis;
using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Ardalis.GuardClauses;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Ages.GetAges;

[ExcludeFromCodeCoverage]
public record GetAgesQuery : IRequest<IList<DropdownDataResponse<string>>>;

public class GetAgesQueryHandler : IRequestHandler<GetAgesQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IMemoryCache _memoryCache;

    public GetAgesQueryHandler(IMemoryCache memoryCache)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
    }

    public Task<IList<DropdownDataResponse<string>>> Handle(GetAgesQuery request, CancellationToken cancellationToken)
    {
        var ages = _memoryCache.GetOrCreate(CacheKeys.Ages, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            IList<DropdownDataResponse<string>> allAges = [];
            foreach (Age age in Enum.GetValues<Age>())
            {
                allAges.Add(new DropdownDataResponse<string>()
                {
                    Label = Enum.GetName(typeof(Age), age)!,
                    Value = ((int)age).ToString()
                });
            }

            return allAges;
        }) ?? [];

        return Task.FromResult(ages);
    }
}