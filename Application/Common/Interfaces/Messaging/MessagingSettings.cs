using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.Messaging;

[ExcludeFromCodeCoverage]
public sealed class MessagingSettings
{
    public string From { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Host { get; init; } = null!;
    public int Port { get; init; }
}