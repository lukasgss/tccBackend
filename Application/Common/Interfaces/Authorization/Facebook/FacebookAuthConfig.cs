using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.Authorization.Facebook;

[ExcludeFromCodeCoverage]
public class FacebookAuthConfig
{
	public required string AppId { get; set; }
	public required string AppSecret { get; set; }
}