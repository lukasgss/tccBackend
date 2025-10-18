namespace Application.Common.Interfaces.ExternalServices;

public class AwsS3FileResponse
{
    public bool Success { get; init; }
    public string? PublicUrl { get; init; }
}