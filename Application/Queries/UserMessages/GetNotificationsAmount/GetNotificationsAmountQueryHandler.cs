using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.UserMessages.GetNotificationsAmount;

[ExcludeFromCodeCoverage]
public record GetNotificationsAmountQuery(Guid UserId) : IRequest<NotificationAmountResponse>;

public class GetNotificationsAmountQueryHandler
    : IRequestHandler<GetNotificationsAmountQuery, NotificationAmountResponse>
{
    private readonly IAppDbContext _dbContext;

    public GetNotificationsAmountQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<NotificationAmountResponse> Handle(GetNotificationsAmountQuery request,
        CancellationToken cancellationToken)
    {
        int notificationsAmount = await _dbContext.UserMessages
            .Where(message => message.SenderId != request.UserId && message.ReceiverId == request.UserId &&
                              !message.HasBeenRead)
            .GroupBy(message => message.Sender)
            .CountAsync(cancellationToken);

        return new NotificationAmountResponse(notificationsAmount);
    }
}