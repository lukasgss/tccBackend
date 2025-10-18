using Application.Common.Interfaces.General.Files;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Pets.UploadImages;

public record UploadPetImagesCommand(Pet Pet, List<IFormFile> Images) : IRequest<List<PetImage>>;

public class UploadPetImagesCommandHandler : IRequestHandler<UploadPetImagesCommand, List<PetImage>>
{
    private readonly IPetImageSubmissionService _petImageSubmissionService;

    public UploadPetImagesCommandHandler(IPetImageSubmissionService petImageSubmissionService)
    {
        _petImageSubmissionService = Guard.Against.Null(petImageSubmissionService);
    }

    public async Task<List<PetImage>> Handle(UploadPetImagesCommand request,
        CancellationToken cancellationToken)
    {
        var uploadedImageUrls = await _petImageSubmissionService.UploadPetImageAsync(request.Images);

        return uploadedImageUrls
            .Select(image => new PetImage() { ImageUrl = image, PetId = request.Pet.Id, Pet = request.Pet })
            .ToList();
    }
}