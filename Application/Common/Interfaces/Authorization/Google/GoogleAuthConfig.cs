using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.Authorization.Google;

[ExcludeFromCodeCoverage]
public class GoogleAuthConfig
{
	public required string ClientId { get; set; } = null!;
	public required string ClientSecret { get; set; } = null!;
}