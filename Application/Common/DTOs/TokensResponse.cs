using System.Diagnostics.CodeAnalysis;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed class TokensResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}