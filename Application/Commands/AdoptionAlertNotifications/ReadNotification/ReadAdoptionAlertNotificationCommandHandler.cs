using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AdoptionAlertNotifications;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts.Notifications;
using MediatR;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionAlertNotifications.ReadNotification;

public record ReadAdoptionAlertNotificationCommand(long NotificationId, Guid UserId)
    : IRequest<Unit>;

public class ReadAdoptionAlertNotificationCommandHandler : IRequestHandler<ReadAdoptionAlertNotificationCommand, Unit>
{
    private readonly IAdoptionAlertNotificationsRepository _adoptionAlertNotificationsRepository;

    public ReadAdoptionAlertNotificationCommandHandler(
        IAdoptionAlertNotificationsRepository adoptionAlertNotificationsRepository)
    {
        _adoptionAlertNotificationsRepository = Guard.Against.Null(adoptionAlertNotificationsRepository);
    }

    public async Task<Unit> Handle(ReadAdoptionAlertNotificationCommand request, CancellationToken cancellationToken)
    {
        AdoptionAlertNotification? notification =
            await _adoptionAlertNotificationsRepository.GetByIdAsync(request.NotificationId);
        if (notification is null)
        {
            throw new NotFoundException("Notificação não foi encontrada.");
        }

        if (notification.Users.All(user => user.Id != request.UserId))
        {
            throw new UnauthorizedException("Não é possível ler notificações que não são para você.");
        }

        notification.HasBeenRead = true;
        await _adoptionAlertNotificationsRepository.CommitAsync();

        return Unit.Value;
    }
}