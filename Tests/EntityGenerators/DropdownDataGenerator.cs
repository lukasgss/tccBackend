using System.Collections.Generic;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class DropdownDataGenerator
{
    public static List<DropdownDataResponse<int>> GenerateDropdownDataResponsesOfBreeds(List<Breed> breeds)
    {
        List<DropdownDataResponse<int>> dropdownDataResponses = new();
        foreach (Breed breed in breeds)
        {
            dropdownDataResponses.Add(new DropdownDataResponse<int>()
            {
                Label = breed.Name,
                Value = breed.Id
            });
        }

        return dropdownDataResponses;
    }

    public static List<DropdownDataResponse<int>> GenerateDropdownDataResponsesOfSpecies(List<Species> species)
    {
        List<DropdownDataResponse<int>> dropdownDataResponses = new();
        foreach (Species speciesValue in species)
        {
            dropdownDataResponses.Add(new DropdownDataResponse<int>()
            {
                Label = speciesValue.Name,
                Value = speciesValue.Id
            });
        }

        return dropdownDataResponses;
    }
}