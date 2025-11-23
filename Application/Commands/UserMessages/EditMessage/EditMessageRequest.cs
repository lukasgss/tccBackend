using System.Diagnostics.CodeAnalysis;

namespace Application.Commands.UserMessages.EditMessage;

[ExcludeFromCodeCoverage]
public sealed record EditMessageRequest(string Content);