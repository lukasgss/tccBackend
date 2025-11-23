using Application.Commands.AdoptionFavorites.ToggleFavorite;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;
using Domain.Enums;
using Domain.ValueObjects;

namespace Tests.AdoptionAlerts;

public sealed class ToggleFavoriteCommandHandlerTests
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IAdoptionFavoritesRepository _adoptionFavoritesRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;
	private readonly ToggleFavoriteCommandHandler _handler;

	public ToggleFavoriteCommandHandlerTests()
	{
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_adoptionFavoritesRepository = Substitute.For<IAdoptionFavoritesRepository>();
		_userRepository = Substitute.For<IUserRepository>();
		_valueProvider = Substitute.For<IValueProvider>();

		_handler = new ToggleFavoriteCommandHandler(
			_adoptionAlertRepository,
			_adoptionFavoritesRepository,
			_userRepository,
			_valueProvider);
	}

	[Fact]
	public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new ToggleFavoriteCommand(Guid.NewGuid(), Guid.NewGuid());
		_adoptionAlertRepository.GetByIdAsync(command.AlertId).Returns((AdoptionAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenFavoriteDoesNotExist_ShouldAddNewFavorite()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var alertId = Guid.NewGuid();
		var newFavoriteId = Guid.NewGuid();
		var command = new ToggleFavoriteCommand(userId, alertId);

		var adoptionAlert = new AdoptionAlert
		{
			Id = alertId,
			Neighborhood = "neighborhood",
			Pet = new Pet
			{
				Id = Guid.NewGuid(),
				Name = "Test Pet",
				Age = Age.Adulto,
				Size = Size.Grande,
				Gender = Gender.Macho,
				Images = new List<PetImage>()
			}
		};
		var user = new User { Id = userId };

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_adoptionFavoritesRepository.GetFavoriteAlertAsync(userId, alertId).Returns((AdoptionFavorite?)null);
		_valueProvider.NewGuid().Returns(newFavoriteId);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		_adoptionFavoritesRepository.Received(1).Add(Arg.Is<AdoptionFavorite>(f =>
			f.Id == newFavoriteId &&
			f.UserId == userId &&
			f.AdoptionAlertId == alertId));
		await _adoptionFavoritesRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenFavoriteExists_ShouldDeleteFavorite()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var alertId = Guid.NewGuid();
		var command = new ToggleFavoriteCommand(userId, alertId);

		var adoptionAlert = new AdoptionAlert
		{
			Id = alertId,
			Neighborhood = "neighborhood",
			Pet = new Pet
			{
				Id = Guid.NewGuid(),
				Name = "Test Pet",
				Age = Age.Adulto,
				Size = Size.Médio,
				Gender = Gender.Macho,
				Images = new List<PetImage>()
			}
		};
		var user = new User { Id = userId };
		var existingFavorite = new AdoptionFavorite
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			AdoptionAlertId = alertId,
			User = user,
			AdoptionAlert = adoptionAlert
		};

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_adoptionFavoritesRepository.GetFavoriteAlertAsync(userId, alertId).Returns(existingFavorite);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		_adoptionFavoritesRepository.Received(1).Delete(existingFavorite);
		await _adoptionFavoritesRepository.Received(1).CommitAsync();
		_adoptionFavoritesRepository.DidNotReceive().Add(Arg.Any<AdoptionFavorite>());
	}
}