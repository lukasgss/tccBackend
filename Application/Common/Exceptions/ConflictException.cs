using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class ConflictException : Exception
{
	public ConflictException()
	{
	}

	public ConflictException(string message) : base(message)
	{
	}

	public ConflictException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}