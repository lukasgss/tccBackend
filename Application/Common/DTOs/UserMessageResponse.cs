using Application.Queries.Users.Common;

namespace Application.Common.DTOs;

public record UserMessageResponse(
    long Id,
    string Content,
    DateTime TimeStampUtc,
    bool HasBeenRead,
    bool HasBeenEdited,
    bool HasBeenDeleted,
    UserDataResponse Sender,
    UserDataResponse Receiver
);