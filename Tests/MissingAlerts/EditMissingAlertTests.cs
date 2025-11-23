using Application.Commands.MissingAlerts.Edit;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Tests.MissingAlerts;

public sealed class EditMissingAlertCommandHandlerTests
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IPetRepository _petRepository;
	private readonly IUserRepository _userRepository;
	private readonly EditMissingAlertCommandHandler _handler;

	public EditMissingAlertCommandHandlerTests()
	{
		_missingAlertRepository = Substitute.For<IMissingAlertRepository>();
		_petRepository = Substitute.For<IPetRepository>();
		_userRepository = Substitute.For<IUserRepository>();
		var logger = Substitute.For<ILogger<EditMissingAlertCommandHandler>>();

		_handler = new EditMissingAlertCommandHandler(
			_missingAlertRepository,
			_petRepository,
			_userRepository,
			logger);
	}

	[Fact]
	public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		_missingAlertRepository.GetByIdAsync(command.Id).Returns((MissingAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenPetNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		var missingAlert = CreateMissingAlert(command.Id, command.UserId);

		_missingAlertRepository.GetByIdAsync(command.Id).Returns(missingAlert);
		_petRepository.GetPetByIdAsync(command.PetId).Returns((Pet?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Animal com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldThrowForbiddenException()
	{
		// Arrange
		var command = CreateValidCommand();
		var differentOwnerId = Guid.NewGuid();
		var missingAlert = CreateMissingAlert(command.Id, differentOwnerId);

		_missingAlertRepository.GetByIdAsync(command.Id).Returns(missingAlert);
		_petRepository.GetPetByIdAsync(command.PetId).Returns(CreatePet(command.PetId));

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível editar alertas de outros usuários.");
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		var missingAlert = CreateMissingAlert(command.Id, command.UserId);

		_missingAlertRepository.GetByIdAsync(command.Id).Returns(missingAlert);
		_petRepository.GetPetByIdAsync(command.PetId).Returns(CreatePet(command.PetId));
		_userRepository.GetUserByIdAsync(command.UserId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Usuário com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldUpdateAlertAndReturnResponse()
	{
		// Arrange
		var command = CreateValidCommand();
		var missingAlert = CreateMissingAlert(command.Id, command.UserId);
		var pet = CreatePet(command.PetId);
		var user = new User { Id = command.UserId };

		_missingAlertRepository.GetByIdAsync(command.Id).Returns(missingAlert);
		_petRepository.GetPetByIdAsync(command.PetId).Returns(pet);
		_userRepository.GetUserByIdAsync(command.UserId).Returns(user);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		missingAlert.Description.ShouldBe(command.Description);
		missingAlert.Pet.ShouldBe(pet);
		missingAlert.User.ShouldBe(user);
		await _missingAlertRepository.Received(1).CommitAsync();
	}

	private static EditMissingAlertCommand CreateValidCommand()
	{
		return new EditMissingAlertCommand(
			Id: Guid.NewGuid(),
			LastSeenLocationLatitude: -23.5505,
			LastSeenLocationLongitude: -46.6333,
			Description: "Test Description",
			PetId: Guid.NewGuid(),
			UserId: Guid.NewGuid()
		);
	}

	private static MissingAlert CreateMissingAlert(Guid alertId, Guid ownerId)
	{
		return new MissingAlert
		{
			Id = alertId,
			User = new User { Id = ownerId },
			Location = new Point(0, 0),
			RegistrationDate = DateTime.UtcNow,
			Neighborhood = "Test Neighborhood",
			Pet = CreatePet(Guid.NewGuid()),
			State = new State(1, "Test State"),
			City = new City(1, "Test City")
		};
	}

	private static Pet CreatePet(Guid petId)
	{
		return new Pet
		{
			Id = petId,
			Name = "Test Pet",
			Age = Age.Adulto,
			Size = Size.Médio,
			Gender = Gender.Macho,
			Images = new List<PetImage>(),
			Colors = new List<Color>(),
			Species = new Species { Id = 1, Name = "Test Species" },
			Breed = new Breed { Id = 1, Name = "Test Breed" }
		};
	}
}