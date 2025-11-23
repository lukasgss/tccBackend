using Application.Commands.FoundAlertFavorites.ToggleFavorite;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.FoundAnimalFavoriteAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;
using Domain.Enums;
using Domain.ValueObjects;

namespace Tests.FoundAlerts;

public sealed class ToggleFoundAlertFavoriteCommandHandlerTests
{
    private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepository;
    private readonly IFoundAnimalFavoritesRepository _foundAnimalFavoritesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ToggleFoundAlertFavoriteCommandHandler _handler;

    public ToggleFoundAlertFavoriteCommandHandlerTests()
    {
        _foundAnimalAlertRepository = Substitute.For<IFoundAnimalAlertRepository>();
        _foundAnimalFavoritesRepository = Substitute.For<IFoundAnimalFavoritesRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _valueProvider = Substitute.For<IValueProvider>();

        _handler = new ToggleFoundAlertFavoriteCommandHandler(
            _foundAnimalAlertRepository,
            _userRepository,
            _valueProvider,
            _foundAnimalFavoritesRepository);
    }

    [Fact]
    public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new ToggleFoundAlertFavoriteCommand(Guid.NewGuid(), Guid.NewGuid());
        _foundAnimalAlertRepository.GetByIdAsync(command.AlertId).Returns((FoundAnimalAlert?)null);

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
        var command = new ToggleFoundAlertFavoriteCommand(userId, alertId);

        var foundAnimalAlert = CreateFoundAnimalAlert(alertId);
        var user = new User { Id = userId };

        _foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(foundAnimalAlert);
        _userRepository.GetUserByIdAsync(userId).Returns(user);
        _foundAnimalFavoritesRepository.GetFavoriteAlertAsync(userId, alertId).Returns((FoundAnimalFavorite?)null);
        _valueProvider.NewGuid().Returns(newFavoriteId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        _foundAnimalFavoritesRepository.Received(1).Add(Arg.Is<FoundAnimalFavorite>(f =>
            f.Id == newFavoriteId &&
            f.UserId == userId &&
            f.FoundAnimalAlertId == alertId));
        await _foundAnimalFavoritesRepository.Received(1).CommitAsync();
    }

    [Fact]
    public async Task Handle_WhenFavoriteExists_ShouldDeleteFavorite()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var alertId = Guid.NewGuid();
        var command = new ToggleFoundAlertFavoriteCommand(userId, alertId);

        var foundAnimalAlert = CreateFoundAnimalAlert(alertId);
        var user = new User { Id = userId };
        var existingFavorite = new FoundAnimalFavorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FoundAnimalAlertId = alertId,
            User = user,
            FoundAnimalAlert = foundAnimalAlert
        };

        _foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(foundAnimalAlert);
        _userRepository.GetUserByIdAsync(userId).Returns(user);
        _foundAnimalFavoritesRepository.GetFavoriteAlertAsync(userId, alertId).Returns(existingFavorite);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        _foundAnimalFavoritesRepository.Received(1).Delete(existingFavorite);
        await _foundAnimalFavoritesRepository.Received(1).CommitAsync();
        _foundAnimalFavoritesRepository.DidNotReceive().Add(Arg.Any<FoundAnimalFavorite>());
    }

    private static FoundAnimalAlert CreateFoundAnimalAlert(Guid alertId)
    {
        return new FoundAnimalAlert
        {
            Id = alertId,
            Neighborhood = "Test Neighborhood",
            Age = Age.Adulto,
            Size = Size.Médio,
            Gender = Gender.Macho,
            RegistrationDate = DateTime.UtcNow,
            Images = new List<FoundAnimalAlertImage>(),
            Colors = new List<Color>(),
            Species = new Species { Id = 1, Name = "Test Species" }
        };
    }
}
