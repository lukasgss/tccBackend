using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class AgeMappings
{
    extension(Age age)
    {
        public AgeResponse ToAgeResponse()
        {
            return new AgeResponse(age, Enum.GetName(typeof(Age), age)!);
        }
    }

    extension(ICollection<Age>? ages)
    {
        public List<AgeResponse> ToListOfAgeResponse()
        {
            if (ages is null)
            {
                return [];
            }

            return ages.Select(age => new AgeResponse(age, Enum.GetName(typeof(Age), age)!))
                .ToList();
        }
    }
}