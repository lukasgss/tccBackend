using Application.Common.Calculators;
using Application.Common.Exceptions;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Localization;
using Application.Queries.GeoLocation.GetCoordinatesFromStateAndCity;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Application.Commands.Alerts.Common;

public sealed class LocationUtils : ILocationUtils
{
	private readonly ILocalizationRepository _localizationRepository;
	private readonly ISender _mediator;
	private readonly ILogger<LocationUtils> _logger;

	public LocationUtils(ILocalizationRepository localizationRepository, ILogger<LocationUtils> logger,
		ISender mediator)
	{
		_localizationRepository = localizationRepository;
		_logger = logger;
		_mediator = mediator;
	}

	public async Task<Point> GetAlertLocation(AlertLocalization localizationData, string neighborhood)
	{
		GetCoordinatesFromStateAndCityQuery query = new(
			neighborhood,
			localizationData.State.Name,
			localizationData.City.Name);
		var addressData = await _mediator.Send(query);

		if (addressData is null)
		{
			throw new NotFoundException("Não foi possível encontrar coordenadas do bairro especificado.");
		}

		return CoordinatesCalculator.CreatePointBasedOnCoordinates(
			double.Parse(addressData.Latitude),
			double.Parse(addressData.Longitude));
	}


	public async Task<AlertLocalization> GetAlertStateAndCity(int stateId, int cityId)
	{
		State state = await GetState(stateId);
		City city = await GetCity(cityId);

		return new AlertLocalization()
		{
			State = state,
			City = city
		};
	}

	private async Task<State> GetState(int stateId)
	{
		var state = await _localizationRepository.GetStateById(stateId);
		if (state is null)
		{
			_logger.LogInformation("Estado de id {EstadoId} não foi encontrado.", stateId);
			throw new NotFoundException("Estado especificado não foi encontrado.");
		}

		return state;
	}

	private async Task<City> GetCity(int cityId)
	{
		var city = await _localizationRepository.GetCityById(cityId);
		if (city is null)
		{
			_logger.LogInformation("Cidade de id {CidadeId} não foi encontrada.", cityId);
			throw new NotFoundException("Cidade especificada não foi encontrada.");
		}

		return city;
	}
}