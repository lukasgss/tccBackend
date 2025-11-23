using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class ForbiddenException : Exception
{
	public ForbiddenException()
	{
	}

	public ForbiddenException(string message) : base(message)
	{
	}

	public ForbiddenException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}