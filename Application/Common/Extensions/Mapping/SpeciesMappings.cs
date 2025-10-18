using Application.Common.DTOs;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class SpeciesMappings
{
    public static SpeciesResponse ToSpeciesResponse(this Species species)
    {
        return new SpeciesResponse(
            Id: species.Id,
            Name: species.Name
        );
    }

    public static List<SpeciesResponse> ToListOfSpeciesResponse(this ICollection<Species>? species)
    {
        if (species is null)
        {
            return [];
        }

        return species.Select(s => new SpeciesResponse(
                Id: s.Id,
                Name: s.Name
            )
        ).ToList();
    }

    private static DropdownDataResponse<string> ToDropdownData(this Species species)
    {
        return new DropdownDataResponse<string>()
        {
            Label = species.Name,
            Value = species.Id.ToString()
        };
    }

    public static List<DropdownDataResponse<string>> ToListOfDropdownData(this IEnumerable<Species> species)
    {
        List<DropdownDataResponse<string>> dropdownDataSpecies = [];
        foreach (Species speciesValue in species)
        {
            dropdownDataSpecies.Add(speciesValue.ToDropdownData());
        }

        return dropdownDataSpecies;
    }
}