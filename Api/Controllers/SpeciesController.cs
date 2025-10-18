using Application.Common.Interfaces.FrontendDropdownData;
using Application.Queries.Species.GetSpecies;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/species")]
public class SpeciesController : ControllerBase
{
    private readonly ISender _mediator;

    public SpeciesController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("dropdown")]
    public async Task<IList<DropdownDataResponse<string>>> GetAllSpecies()
    {
        return await _mediator.Send(new GetAllSpeciesQuery());
    }
}