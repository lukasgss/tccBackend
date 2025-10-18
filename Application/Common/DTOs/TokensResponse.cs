namespace Application.Common.DTOs;

public class TokensResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}