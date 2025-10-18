using Application.Common.Cache;
using Application.Common.Interfaces.FrontendDropdownData;
using Ardalis.GuardClauses;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.Genders.GetGendersForFilters;

public record GetGendersForFiltersQuery : IRequest<IList<DropdownDataResponse<string>>>;

public class GetGendersForFiltersQueryHandler
    : IRequestHandler<GetGendersForFiltersQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IMemoryCache _memoryCache;

    public GetGendersForFiltersQueryHandler(IMemoryCache memoryCache)
    {
        _memoryCache = Guard.Against.Null(memoryCache);
    }

    public Task<IList<DropdownDataResponse<string>>> Handle(GetGendersForFiltersQuery request,
        CancellationToken cancellationToken)
    {
        var genders = _memoryCache.GetOrCreate(CacheKeys.Genders, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            IList<DropdownDataResponse<string>> allGenders = [];
            foreach (Gender gender in Enum.GetValues<Gender>())
            {
                if (gender == Gender.Desconhecido)
                {
                    continue;
                }

                allGenders.Add(new DropdownDataResponse<string>()
                {
                    Label = Enum.GetName(typeof(Gender), gender)!,
                    Value = ((int)gender).ToString()
                });
            }

            return allGenders;
        }) ?? [];

        return Task.FromResult(genders);
    }
}