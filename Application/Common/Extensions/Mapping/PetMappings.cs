using Application.Common.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Common.Extensions.Mapping;

public static class PetMappings
{
    public static PetResponse ToPetResponse(this Pet pet)
    {
        return new PetResponse(
            Id: pet.Id,
            Name: pet.Name,
            Age: pet.Age.ToAgeResponse(),
            Size: pet.Size.ToSizeResponse(),
            Images: pet.Images.ToPetImagesResponse(),
            Gender: pet.Gender.ToGenderResponse(),
            Owner: pet.Owner.ToOwnerResponse(),
            Breed: pet.Breed.ToBreedResponse(),
            Colors: pet.Colors.ToListOfColorResponse(),
            Species: pet.Species.ToSpeciesResponse()
        );
    }

    public static ExtraSimplifiedPetResponse ToExtraSimplifiedPetResponse(this Pet pet)
    {
        return new ExtraSimplifiedPetResponse(
            pet.Id,
            pet.Name,
            pet.Age.ToAgeResponse(),
            pet.Breed.ToBreedResponse(),
            pet.Gender.ToGenderResponse(),
            pet.Images.ToPetImagesResponse());
    }

    public static PetResponseNoOwner ToPetResponseNoOwner(this Pet pet)
    {
        return new PetResponseNoOwner(
            Id: pet.Id,
            Name: pet.Name,
            Age: new AgeResponse(pet.Age, Enum.GetName(typeof(Age), pet.Age)!),
            Size: new SizeResponse(pet.Size, Enum.GetName(typeof(Size), pet.Size)!),
            Species: pet.Species.ToSpeciesResponse(),
            Images: pet.Images.ToPetImagesResponse(),
            Gender: new GenderResponse(pet.Gender, Enum.GetName(typeof(Gender), pet.Gender)!),
            IsVaccinated: pet.IsVaccinated,
            IsCastrated: pet.IsCastrated,
            IsNegativeToFivFelv: pet.IsNegativeToFivFelv,
            IsNegativeToLeishmaniasis: pet.IsNegativeToLeishmaniasis,
            Breed: pet.Breed.ToBreedResponse(),
            Colors: pet.Colors.ToListOfColorResponse()
        );
    }

    public static SimplifiedPetResponse ToSimplifiedPetResponse(this Pet pet)
    {
        return new SimplifiedPetResponse(
            Id: pet.Id,
            Name: pet.Name,
            Age: Enum.GetName(typeof(Age), pet.Age)!,
            Size: Enum.GetName(typeof(Size), pet.Size)!,
            Images: pet.Images.ToPetImagesResponse(),
            Gender: Enum.GetName(typeof(Gender), pet.Gender)!
        );
    }

    private static List<string> ToPetImagesResponse(this IEnumerable<PetImage> images)
    {
        return images.Select(petImage => petImage.ImageUrl).ToList();
    }
}