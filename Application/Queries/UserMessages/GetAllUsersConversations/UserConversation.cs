namespace Application.Queries.UserMessages.GetAllUsersConversations;

public record UserConversation(
    Guid UserId,
    string UserImageUrl,
    string UserName,
    string LastMessage,
    int NewMessagesQuantity,
    DateTime LastMessageTimeStamp);