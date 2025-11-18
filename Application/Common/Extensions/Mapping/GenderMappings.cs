using System.Diagnostics.CodeAnalysis;
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

    extension(Gender? gender)
    {
        [return: NotNullIfNotNull(nameof(gender))]
        public GenderResponse? ToGenderResponse()
        {
            if (gender is null)
            {
                return null;
            }

            return new GenderResponse(gender.Value, Enum.GetName(typeof(Gender), gender)!);
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