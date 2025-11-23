using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.ExternalServices;

[ExcludeFromCodeCoverage]
public sealed class AwsS3FileResponse
{
    public bool Success { get; init; }
    public string? PublicUrl { get; init; }
}