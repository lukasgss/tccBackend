using Application.Queries.GeoLocation.Common;
using Application.Queries.GeoLocation.GetCoordinatesFromPostalCode;
using Application.Queries.GeoLocation.GetLocationDataFromCoordinates;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/geo-location")]
public class GeoLocationController : ControllerBase
{
    private readonly ISender _mediator;

    public GeoLocationController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("coordinates/postal-code")]
    public async Task<GeoLocationResponse> GetCoordinatesFromPostalCode(string postalCode)
    {
        GetCoordinatesFromPostalCodeQuery query = new(postalCode);
        return await _mediator.Send(query);
    }

    [HttpGet("address/coordinates")]
    public async Task<GeoLocationResponse> GetAddressFromCoordinates(double latitude, double longitude)
    {
        GetLocationDataFromCoordinatesQuery query = new(latitude, longitude);
        return await _mediator.Send(query);
    }
}