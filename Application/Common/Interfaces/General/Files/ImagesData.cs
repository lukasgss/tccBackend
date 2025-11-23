using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.General.Files;

[ExcludeFromCodeCoverage]
public sealed class ImagesData
{
    public required string DefaultUserProfilePicture { get; init; }
}