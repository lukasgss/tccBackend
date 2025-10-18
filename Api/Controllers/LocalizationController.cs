using Application.Common.Interfaces.FrontendDropdownData;
using Application.Queries.Localization.GetAllCitiesFromState;
using Application.Queries.Localization.GetAllStates;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/localization")]
public class LocalizationController : ControllerBase
{
    private readonly ISender _mediator;

    public LocalizationController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("states")]
    public async Task<IList<DropdownDataResponse<string>>> GetAllStates()
    {
        return await _mediator.Send(new GetAllStatesQuery());
    }

    [HttpGet("cities/{stateId:int}")]
    public async Task<IList<DropdownDataResponse<string>>> GetAllCitiesFromState(int stateId)
    {
        GetAllCitiesFromStateQuery query = new(stateId);
        return await _mediator.Send(query);
    }
}