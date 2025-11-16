using Application.Commands.UserMessages.MarkAllMessagesAsRead;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Persistence;
using Application.Common.Pagination;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.UserMessages.GetMessagesFromUser;

public record GetMessagesFromUserQuery(
    Guid CurrentUserId,
    Guid OtherUserId,
    int PageNumber,
    int PageSize)
    : IRequest<PaginatedEntity<UserMessageResponse>>;

public class
    GetMessagesFromUserQueryHandler : IRequestHandler<GetMessagesFromUserQuery, PaginatedEntity<UserMessageResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ISender _mediator;

    public GetMessagesFromUserQueryHandler(IAppDbContext dbContext, ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<PaginatedEntity<UserMessageResponse>> Handle(GetMessagesFromUserQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await GetAllFromUserAsync(request);

        MarkAllMessagesAsReadCommand command = new(request.OtherUserId, request.CurrentUserId);
        await _mediator.Send(command, cancellationToken);

        return messages.ToUserMessageResponsePagedList();
    }

    private async Task<PagedList<UserMessage>> GetAllFromUserAsync(GetMessagesFromUserQuery request)
    {
        var query = _dbContext.UserMessages
            .AsNoTracking()
            .Select(message => new UserMessage
            {
                Id = message.Id,
                Content = message.Content,
                TimeStampUtc = message.TimeStampUtc,
                HasBeenRead = message.HasBeenRead,
                HasBeenEdited = message.HasBeenEdited,
                HasBeenDeleted = message.HasBeenDeleted,
                Sender = new User
                {
                    Id = message.Sender.Id,
                    Email = message.Sender.Email,
                    FullName = message.Sender.FullName,
                    Image = message.Sender.Image
                },
                SenderId = message.Sender.Id,
                Receiver = new User
                {
                    Id = message.Receiver.Id,
                    Email = message.Receiver.Email,
                    FullName = message.Receiver.FullName,
                    Image = message.Receiver.Image
                },
                ReceiverId = message.Receiver.Id
            })
            .Where(message => (message.Sender.Id == request.OtherUserId
                               && message.Receiver.Id == request.CurrentUserId
                               || message.Sender.Id == request.CurrentUserId
                               && message.Receiver.Id == request.OtherUserId)
                              && !message.HasBeenDeleted)
            .OrderByDescending(message => message.TimeStampUtc);

        return await PagedList<UserMessage>.ToPagedListAsync(query, request.PageNumber, request.PageSize);
    }
}