using Application.Common.DTOs;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class SpeciesMappings
{
    extension(Species species)
    {
        public SpeciesResponse ToSpeciesResponse()
        {
            return new SpeciesResponse(
                species.Id,
                species.Name
            );
        }

        private DropdownDataResponse<string> ToDropdownData()
        {
            return new DropdownDataResponse<string>
            {
                Label = species.Name,
                Value = species.Id.ToString()
            };
        }
    }

    extension(ICollection<Species>? species)
    {
        public List<SpeciesResponse> ToListOfSpeciesResponse()
        {
            if (species is null) return [];

            return species.Select(s => new SpeciesResponse(
                    s.Id,
                    s.Name
                )
            ).ToList();
        }
    }

    extension(IEnumerable<Species> species)
    {
        public List<DropdownDataResponse<string>> ToListOfDropdownData()
        {
            List<DropdownDataResponse<string>> dropdownDataSpecies = [];
            foreach (Species speciesValue in species) dropdownDataSpecies.Add(speciesValue.ToDropdownData());

            return dropdownDataSpecies;
        }
    }
}