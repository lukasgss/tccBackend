using Application.Common.DTOs;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class BreedMappings
{
    extension(Breed breed)
    {
        public BreedResponse ToBreedResponse()
        {
            return new BreedResponse(
                Id: breed.Id,
                Name: breed.Name
            );
        }

        private DropdownDataResponse<string> ToDropdownData()
        {
            return new DropdownDataResponse<string>()
            {
                Label = breed.Name,
                Value = breed.Id.ToString()
            };
        }
    }

    extension(IEnumerable<Breed>? breeds)
    {
        public List<BreedResponse> ToListOfBreedResponse()
        {
            if (breeds is null)
            {
                return [];
            }

            return breeds.Select(breed => new BreedResponse(
                    Id: breed.Id,
                    Name: breed.Name
                ))
                .ToList();
        }

        public List<DropdownDataResponse<string>> ToListOfDropdownData()
        {
            if (breeds is null)
            {
                return [];
            }
            
            List<DropdownDataResponse<string>> dropdownDataBreeds = [];
            foreach (Breed breed in breeds)
            {
                dropdownDataBreeds.Add(breed.ToDropdownData());
            }

            return dropdownDataBreeds;
        }
    }
}