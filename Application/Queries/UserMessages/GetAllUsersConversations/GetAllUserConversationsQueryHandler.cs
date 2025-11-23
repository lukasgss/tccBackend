using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.UserMessages.GetAllUsersConversations;

[ExcludeFromCodeCoverage]
public record GetAllUserConversationsQuery(Guid UserId) : IRequest<IList<UserConversation>>;

public class
    GetAllUserConversationsQueryHandler : IRequestHandler<GetAllUserConversationsQuery, IList<UserConversation>>
{
    private readonly IAppDbContext _dbContext;

    public GetAllUserConversationsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<UserConversation>> Handle(GetAllUserConversationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.UserMessages
            .AsNoTracking()
            .Where(message => !message.HasBeenDeleted &&
                              (message.ReceiverId == request.UserId || message.SenderId == request.UserId))
            .GroupBy(message => message.SenderId == request.UserId ? message.ReceiverId : message.SenderId)
            .Select(group => new UserConversation(
                group.Key,
                group.OrderByDescending(m => m.TimeStampUtc).First().SenderId == request.UserId
                    ? group.OrderByDescending(m => m.TimeStampUtc).First().Receiver.Image
                    : group.OrderByDescending(m => m.TimeStampUtc).First().Sender.Image,
                group.OrderByDescending(m => m.TimeStampUtc).First().SenderId == request.UserId
                    ? group.OrderByDescending(m => m.TimeStampUtc).First().Receiver.FullName
                    : group.OrderByDescending(m => m.TimeStampUtc).First().Sender.FullName,
                group.OrderByDescending(m => m.TimeStampUtc).First().Content,
                group.Count(m => m.ReceiverId == request.UserId && !m.HasBeenRead),
                group.OrderByDescending(m => m.TimeStampUtc).First().TimeStampUtc
            ))
            .ToListAsync(cancellationToken);
    }
}