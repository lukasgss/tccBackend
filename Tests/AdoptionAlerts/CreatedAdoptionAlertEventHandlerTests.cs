using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AdoptionAlertNotifications;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Events.AdoptionAlerts.CreatedAdoptionAlert;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.Notifications;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Tests.AdoptionAlerts;

public sealed class CreatedAdoptionAlertEventHandlerTests
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IAdoptionAlertNotificationsRepository _adoptionAlertNotificationsRepository;
	private readonly IAdoptionUserPreferencesRepository _adoptionUserPreferencesRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;
	private readonly CreatedAdoptionAlertEventHandler _handler;

	public CreatedAdoptionAlertEventHandlerTests()
	{
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_adoptionAlertNotificationsRepository = Substitute.For<IAdoptionAlertNotificationsRepository>();
		_adoptionUserPreferencesRepository = Substitute.For<IAdoptionUserPreferencesRepository>();
		_userRepository = Substitute.For<IUserRepository>();
		_valueProvider = Substitute.For<IValueProvider>();

		_handler = new CreatedAdoptionAlertEventHandler(
			_adoptionAlertNotificationsRepository,
			_adoptionUserPreferencesRepository,
			_adoptionAlertRepository,
			_valueProvider,
			_userRepository);
	}

	[Fact]
	public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var notification = CreateNotification();
		_adoptionAlertRepository.GetByIdAsync(notification.Id).Returns((AdoptionAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(notification, CancellationToken.None));
		exception.Message.ShouldBe("Adoção com o id especificado não foi encontrada.");
	}

	[Fact]
	public async Task Handle_WhenNoUsersMatchPreferences_ShouldReturnWithoutCreatingNotification()
	{
		// Arrange
		var notification = CreateNotification();
		var alert = CreateAdoptionAlert(notification.Id);

		_adoptionAlertRepository.GetByIdAsync(notification.Id).Returns(alert);
		_adoptionUserPreferencesRepository.GetUsersThatMatchPreferences(notification)
			.Returns(new List<UserThatMatchPreferences>());
		_userRepository.GetUsersByIdsAsync(Arg.Any<List<Guid>>())
			.Returns(new List<User>());

		// Act
		await _handler.Handle(notification, CancellationToken.None);

		// Assert
		_adoptionAlertNotificationsRepository.DidNotReceive().Add(Arg.Any<AdoptionAlertNotification>());
		await _adoptionAlertNotificationsRepository.DidNotReceive().CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenUsersMatchPreferences_ShouldCreateNotification()
	{
		// Arrange
		var notification = CreateNotification();
		var alert = CreateAdoptionAlert(notification.Id);
		var now = DateTime.UtcNow;

		var userId1 = Guid.NewGuid();
		var userId2 = Guid.NewGuid();
		var matchingPreferences = new List<UserThatMatchPreferences>
		{
			new(userId1),
			new(userId2)
		};
		var users = new List<User>
		{
			CreateUser(userId1),
			CreateUser(userId2)
		};

		_adoptionAlertRepository.GetByIdAsync(notification.Id).Returns(alert);
		_adoptionUserPreferencesRepository.GetUsersThatMatchPreferences(notification)
			.Returns(matchingPreferences);
		_userRepository.GetUsersByIdsAsync(Arg.Any<List<Guid>>()).Returns(users);
		_valueProvider.UtcNow().Returns(now);

		// Act
		await _handler.Handle(notification, CancellationToken.None);

		// Assert
		_adoptionAlertNotificationsRepository.Received(1).Add(Arg.Is<AdoptionAlertNotification>(n =>
			n.AdoptionAlert == alert &&
			n.TimeStampUtc == now &&
			n.Users.Count == 2));
		await _adoptionAlertNotificationsRepository.Received(1).CommitAsync();
	}

	private static AdoptionAlertCreated CreateNotification()
	{
		return new AdoptionAlertCreated(
			Id: Guid.NewGuid(),
			Gender: Gender.Macho,
			Age: Age.Adulto,
			Size: Size.Médio,
			FoundLocationLatitude: -23.5505,
			FoundLocationLongitude: -46.6333,
			SpeciesId: 1,
			BreedId: 1,
			ColorIds: new List<int> { 1, 2 },
			IsInSameTransaction: false,
			OwnerId: Guid.NewGuid()
		);
	}


	private static AdoptionAlert CreateAdoptionAlert(Guid alertId)
	{
		return new AdoptionAlert
		{
			Id = alertId,
			Neighborhood = "Test Neighborhood",
			Pet = new Pet
			{
				Id = Guid.NewGuid(),
				Name = "Test Pet",
				Age = Age.Adulto,
				Size = Size.Médio,
				Gender = Gender.Macho,
				Images = new List<PetImage>(),
				Colors = new List<Color>(),
				Species = new Species { Id = 1, Name = "Test Species" },
				Breed = new Breed { Id = 1, Name = "Test Breed" }
			},
			User = new User
			{
				Id = Guid.NewGuid(),
				Email = "owner@email.com",
				FullName = "Owner Name",
				PhoneNumber = "123456789",
				Image = "image.jpg",
				ReceivesOnlyWhatsAppMessages = true,
				DefaultAdoptionFormUrl = "url"
			}
		};
	}

	private static User CreateUser(Guid userId)
	{
		return new User
		{
			Id = userId,
			Email = $"user{userId}@email.com",
			ReceivesOnlyWhatsAppMessages = true,
			PhoneNumber = "123456789",
			FullName = "Test User",
			Image = "image.jpg",
			DefaultAdoptionFormUrl = "url"
		};
	}
}