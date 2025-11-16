using Application.Common.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMessageMappings
{
    extension(UserMessage userMessage)
    {
        public UserMessageResponse ToUserMessageResponse()
        {
            return new UserMessageResponse(
                userMessage.Id,
                userMessage.Content,
                userMessage.TimeStampUtc,
                userMessage.HasBeenRead,
                userMessage.HasBeenEdited,
                userMessage.HasBeenDeleted,
                userMessage.Sender.ToUserDataResponse(),
                userMessage.Receiver.ToUserDataResponse()
            );
        }
    }
}