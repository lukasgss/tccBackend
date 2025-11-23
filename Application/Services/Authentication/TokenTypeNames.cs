using System.Diagnostics.CodeAnalysis;

namespace Application.Services.Authentication;

[ExcludeFromCodeCoverage]
public static class TokenTypeNames
{
	public const string AccessToken = "access";
	public const string RefreshToken = "refresh";
}