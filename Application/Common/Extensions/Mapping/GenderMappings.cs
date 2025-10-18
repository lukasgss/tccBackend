using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class GenderMappings
{
    public static GenderResponse ToGenderResponse(this Gender gender)
    {
        return new GenderResponse(gender, Enum.GetName(typeof(Gender), gender)!);
    }

    public static List<GenderResponse> ToListOfGenderResponse(this ICollection<Gender>? genders)
    {
        if (genders is null)
        {
            return [];
        }

        return genders.Select(gender => new GenderResponse(gender, Enum.GetName(typeof(Gender), gender)!))
            .ToList();
    }
}