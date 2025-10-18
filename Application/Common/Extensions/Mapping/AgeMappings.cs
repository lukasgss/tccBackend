using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class AgeMappings
{
    public static AgeResponse ToAgeResponse(this Age age)
    {
        return new AgeResponse(age, Enum.GetName(typeof(Age), age)!);
    }

    public static List<AgeResponse> ToListOfAgeResponse(this ICollection<Age>? ages)
    {
        if (ages is null)
        {
            return [];
        }

        return ages.Select(age => new AgeResponse(age, Enum.GetName(typeof(Age), age)!))
            .ToList();
    }
}