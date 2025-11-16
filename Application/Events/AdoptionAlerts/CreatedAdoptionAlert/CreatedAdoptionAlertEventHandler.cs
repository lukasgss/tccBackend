using Application.Common.Interfaces.Entities.AdoptionAlertNotifications;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.Notifications;
using Domain.Events;
using MediatR;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Events.AdoptionAlerts.CreatedAdoptionAlert;

public class CreatedAdoptionAlertEventHandler : INotificationHandler<AdoptionAlertCreated>
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly IAdoptionAlertNotificationsRepository _adoptionAlertNotificationsRepository;
    private readonly IAdoptionUserPreferencesRepository _adoptionUserPreferencesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValueProvider _valueProvider;

    public CreatedAdoptionAlertEventHandler(
        IAdoptionAlertNotificationsRepository adoptionAlertNotificationsRepository,
        IAdoptionUserPreferencesRepository adoptionUserPreferencesRepository,
        IAdoptionAlertRepository adoptionAlertRepository,
        IValueProvider valueProvider,
        IUserRepository userRepository)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
        _adoptionAlertNotificationsRepository = Guard.Against.Null(adoptionAlertNotificationsRepository);
        _adoptionUserPreferencesRepository = Guard.Against.Null(adoptionUserPreferencesRepository);
    }

    public async Task Handle(AdoptionAlertCreated notification, CancellationToken cancellationToken)
    {
        AdoptionAlert? adoptionAlert = await _adoptionAlertRepository.GetByIdAsync(notification.Id);
        if (adoptionAlert is null)
        {
            throw new NotFoundException("Adoção com o id especificado não foi encontrada.");
        }

        var usersIdsThatMatchPreference =
            await _adoptionUserPreferencesRepository.GetUsersThatMatchPreferences(notification);
        var users = await _userRepository.GetUsersByIdsAsync(usersIdsThatMatchPreference
            .Select(user => user.UserId)
            .ToList());

        if (users.Count == 0)
        {
            return;
        }

        AdoptionAlertNotification newNotification = new()
        {
            AdoptionAlert = adoptionAlert,
            TimeStampUtc = _valueProvider.UtcNow(),
            Users = users
        };

        _adoptionAlertNotificationsRepository.Add(newNotification);
        await _adoptionAlertNotificationsRepository.CommitAsync();
    }
}