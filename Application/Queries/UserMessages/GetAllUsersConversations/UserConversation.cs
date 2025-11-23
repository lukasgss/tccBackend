using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.UserMessages.GetAllUsersConversations;

[ExcludeFromCodeCoverage]
public record UserConversation(
    Guid UserId,
    string UserImageUrl,
    string UserName,
    string LastMessage,
    int NewMessagesQuantity,
    DateTime LastMessageTimeStamp);