using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.RealTimeCommunication;

[ExcludeFromCodeCoverage]
public sealed class SentMessage
{
	public required long Id { get; init; }
	public required Guid SenderId { get; init; }
	public required Guid ReceiverId { get; init; }
	public required string Content { get; init; }
}