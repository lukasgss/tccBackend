using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class GenderMappings
{
    extension(Gender gender)
    {
        public GenderResponse ToGenderResponse()
        {
            return new GenderResponse(gender, Enum.GetName(typeof(Gender), gender)!);
        }
    }

    extension(ICollection<Gender>? genders)
    {
        public List<GenderResponse> ToListOfGenderResponse()
        {
            if (genders is null) return [];

            return genders.Select(gender => new GenderResponse(gender, Enum.GetName(typeof(Gender), gender)!))
                .ToList();
        }
    }
}