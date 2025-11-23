using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class InternalServerErrorException : Exception
{
	public InternalServerErrorException()
	{
	}

	public InternalServerErrorException(string message) : base(message)
	{
	}

	public InternalServerErrorException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}