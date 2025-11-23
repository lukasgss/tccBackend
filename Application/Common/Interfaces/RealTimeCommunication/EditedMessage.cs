using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.RealTimeCommunication;

[ExcludeFromCodeCoverage]
public sealed class EditedMessage
{
	public required long Id { get; set; }
	public required string Content { get; init; }
	public required Guid SenderId { get; init; }
	public required Guid ReceiverId { get; init; }
}