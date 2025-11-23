using System.Diagnostics.CodeAnalysis;
using Application.Queries.Users.Common;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record UserMessageResponse(
    long Id,
    string Content,
    DateTime TimeStampUtc,
    bool HasBeenRead,
    bool HasBeenEdited,
    bool HasBeenDeleted,
    UserDataResponse Sender,
    UserDataResponse Receiver
);