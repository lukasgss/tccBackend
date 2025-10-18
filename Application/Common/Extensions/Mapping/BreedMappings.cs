using Application.Common.DTOs;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class BreedMappings
{
    public static BreedResponse ToBreedResponse(this Breed breed)
    {
        return new BreedResponse(
            Id: breed.Id,
            Name: breed.Name
        );
    }

    public static List<BreedResponse> ToListOfBreedResponse(this ICollection<Breed>? breeds)
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

    private static DropdownDataResponse<string> ToDropdownData(this Breed breed)
    {
        return new DropdownDataResponse<string>()
        {
            Label = breed.Name,
            Value = breed.Id.ToString()
        };
    }

    public static List<DropdownDataResponse<string>> ToListOfDropdownData(this IEnumerable<Breed> breeds)
    {
        List<DropdownDataResponse<string>> dropdownDataBreeds = [];
        foreach (Breed breed in breeds)
        {
            dropdownDataBreeds.Add(breed.ToDropdownData());
        }

        return dropdownDataBreeds;
    }
}