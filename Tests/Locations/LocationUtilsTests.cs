using Application.Commands.Alerts.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Localization;
using Application.Queries.GeoLocation.Common;
using Application.Queries.GeoLocation.GetCoordinatesFromStateAndCity;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Tests.Locations;

public sealed class LocationUtilsTests
{
	private readonly ILocalizationRepository _localizationRepository;
	private readonly ISender _mediator;
	private readonly LocationUtils _locationUtils;

	public LocationUtilsTests()
	{
		_localizationRepository = Substitute.For<ILocalizationRepository>();
		_mediator = Substitute.For<ISender>();
		var logger = Substitute.For<ILogger<LocationUtils>>();

		_locationUtils = new LocationUtils(
			_localizationRepository,
			logger,
			_mediator);
	}

	[Fact]
	public async Task GetAlertLocation_WhenCoordinatesNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var localizationData = new AlertLocalization
		{
			State = new State(1, "Test State"),
			City = new City(1, "Test City")
		};
		var neighborhood = "Test Neighborhood";

		_mediator.Send(Arg.Any<GetCoordinatesFromStateAndCityQuery>(), Arg.Any<CancellationToken>())
			.Returns((GeoLocationResponse?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() =>
				_locationUtils.GetAlertLocation(localizationData, neighborhood));
		exception.Message.ShouldBe("Não foi possível encontrar coordenadas do bairro especificado.");
	}

	[Fact]
	public async Task GetAlertLocation_WhenCoordinatesFound_ShouldReturnPoint()
	{
		// Arrange
		var localizationData = new AlertLocalization
		{
			State = new State(1, "Test State"),
			City = new City(1, "Test City")
		};
		var neighborhood = "Test Neighborhood";
		var geoLocationResponse = new GeoLocationResponse(
			Latitude: "-23.5505",
			Longitude: "-46.6333",
			Address: null,
			PostalCode: null,
			State: "Test State",
			City: "Test City",
			Neighborhood: neighborhood);

		_mediator.Send(Arg.Any<GetCoordinatesFromStateAndCityQuery>(), Arg.Any<CancellationToken>())
			.Returns(geoLocationResponse);

		// Act
		var result = await _locationUtils.GetAlertLocation(localizationData, neighborhood);

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeOfType<Point>();
	}

	[Fact]
	public async Task GetAlertStateAndCity_WhenStateNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var stateId = 1;
		var cityId = 1;

		_localizationRepository.GetStateById(stateId).Returns((State?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _locationUtils.GetAlertStateAndCity(stateId, cityId));
		exception.Message.ShouldBe("Estado especificado não foi encontrado.");
	}

	[Fact]
	public async Task GetAlertStateAndCity_WhenCityNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var stateId = 1;
		var cityId = 1;

		_localizationRepository.GetStateById(stateId).Returns(new State(stateId, "Test State"));
		_localizationRepository.GetCityById(cityId).Returns((City?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _locationUtils.GetAlertStateAndCity(stateId, cityId));
		exception.Message.ShouldBe("Cidade especificada não foi encontrada.");
	}

	[Fact]
	public async Task GetAlertStateAndCity_WhenStateAndCityFound_ShouldReturnAlertLocalization()
	{
		// Arrange
		var stateId = 1;
		var cityId = 1;
		var state = new State(stateId, "Test State");
		var city = new City(cityId, "Test City");

		_localizationRepository.GetStateById(stateId).Returns(state);
		_localizationRepository.GetCityById(cityId).Returns(city);

		// Act
		var result = await _locationUtils.GetAlertStateAndCity(stateId, cityId);

		// Assert
		result.ShouldNotBeNull();
		result.State.ShouldBe(state);
		result.City.ShouldBe(city);
	}
}