using Application.Common.Interfaces.Entities.AdoptionAlertNotifications;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Commands.AdoptionAlertNotifications.ReadAllNotifications;

public record ReadAllAdoptionNotificationsCommand(Guid UserId) : IRequest<Unit>;

public class ReadAllAdoptionNotificationsCommandHandler : IRequestHandler<ReadAllAdoptionNotificationsCommand, Unit>
{
    private readonly IAdoptionAlertNotificationsRepository _adoptionAlertNotificationsRepository;

    public ReadAllAdoptionNotificationsCommandHandler(
        IAdoptionAlertNotificationsRepository adoptionAlertNotificationsRepository)
    {
        _adoptionAlertNotificationsRepository = Guard.Against.Null(adoptionAlertNotificationsRepository);
    }

    public async Task<Unit> Handle(ReadAllAdoptionNotificationsCommand request, CancellationToken cancellationToken)
    {
        await _adoptionAlertNotificationsRepository.ReadAllAsync(request.UserId);

        return Unit.Value;
    }
}