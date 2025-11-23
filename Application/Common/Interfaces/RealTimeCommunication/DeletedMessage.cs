using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.RealTimeCommunication;

[ExcludeFromCodeCoverage]
public sealed class DeletedMessage
{
	public required long Id { get; init; }
	public required Guid SenderId { get; init; }
	public required Guid ReceiverId { get; init; }
}