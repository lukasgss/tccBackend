using System.Collections.Generic;
using Application.Commands.UserMessages.EditMessage;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Domain.Entities;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators;

public static class UserMessageGenerator
{
    public static UserMessage GenerateUserMessage()
    {
        return new UserMessage()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = Constants.UserMessageData.HasBeenEdited,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender,
            SenderId = Constants.UserMessageData.SenderId,
            Receiver = Constants.UserMessageData.Receiver,
            ReceiverId = Constants.UserMessageData.ReceiverId
        };
    }

    public static UserMessage GenerateEditedUserMessage()
    {
        return new UserMessage()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = true,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender,
            SenderId = Constants.UserMessageData.SenderId,
            Receiver = Constants.UserMessageData.Receiver,
            ReceiverId = Constants.UserMessageData.ReceiverId
        };
    }

    public static List<UserMessage> GenerateListOfUserMessages()
    {
        List<UserMessage> userMessages = new();
        for (int i = 0; i < 3; i++)
        {
            userMessages.Add(GenerateUserMessage());
        }

        return userMessages;
    }

    public static EditMessageRequest GenerateEditUserMessageRequest()
    {
        return new EditMessageRequest(Content: Constants.UserMessageData.Content);
    }

    public static List<UserMessageResponse> GenerateListOfUserMessageResponses()
    {
        List<UserMessageResponse> userMessageResponses = new(3);
        for (int i = 0; i < 3; i++)
        {
            userMessageResponses.Add(GenerateUserMessageResponse());
        }

        return userMessageResponses;
    }

    public static UserMessageResponse GenerateUserMessageResponse()
    {
        return new UserMessageResponse()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = Constants.UserMessageData.HasBeenEdited,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender.ToUserDataResponse(),
            Receiver = Constants.UserMessageData.Receiver.ToUserDataResponse()
        };
    }

    public static UserMessageResponse GenerateEditedUserMessageResponse()
    {
        return new UserMessageResponse()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = true,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender.ToUserDataResponse(),
            Receiver = Constants.UserMessageData.Receiver.ToUserDataResponse()
        };
    }
}