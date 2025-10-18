using Application.Common.Interfaces.ExternalServices.AWS;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Queries.Users.Images.ValidateImage;

public record ValidateUserImageQuery(string Image) : IRequest<string>;

public class ValidateUserImageQueryHandler : IRequestHandler<ValidateUserImageQuery, string>
{
    private readonly IFileUploadClient _fileUploadClient;

    public ValidateUserImageQueryHandler(IFileUploadClient fileUploadClient)
    {
        _fileUploadClient = Guard.Against.Null(fileUploadClient);
    }

    public Task<string> Handle(ValidateUserImageQuery request, CancellationToken cancellationToken)
    {
        const int maxLengthOfImageUrl = 180;

        string imageUrl = request.Image.Length > maxLengthOfImageUrl
            ? _fileUploadClient.FormatPublicUrlString(null)
            : request.Image;

        return Task.FromResult(imageUrl);
    }
}