using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.AdoptionAlertNotifications.GetNotifications;

public record GetAdoptionAlertNotificationsQuery(Guid UserId) : IRequest<List<AdoptionAlertNotificationResponse>>;

public class GetAdoptionAlertNotificationsQueryHandler
    : IRequestHandler<GetAdoptionAlertNotificationsQuery, List<AdoptionAlertNotificationResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetAdoptionAlertNotificationsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<List<AdoptionAlertNotificationResponse>> Handle(GetAdoptionAlertNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext
            .AdoptionAlertNotifications
            .AsNoTracking()
            .Where(notification => notification.Users.Any(user => user.Id == request.UserId))
            .OrderBy(notification => notification.HasBeenRead)
            .ThenByDescending(notification => notification.TimeStampUtc)
            .Select(notification =>
                new AdoptionAlertNotificationResponse(
                    notification.Id,
                    notification.AdoptionAlert.Id,
                    notification.AdoptionAlert.Pet.Images.First().ImageUrl,
                    notification.HasBeenRead,
                    notification.TimeStampUtc))
            .ToListAsync(cancellationToken);
    }
}