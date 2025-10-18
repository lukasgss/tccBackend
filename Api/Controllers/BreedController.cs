using Application.Common.Interfaces.FrontendDropdownData;
using Application.Queries.Breeds.GetBreeds;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/breeds")]
public class BreedController : ControllerBase
{
    private readonly ISender _mediator;

    public BreedController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("dropdown")]
    public async Task<IList<DropdownDataResponse<string>>> GetBreeds(int speciesId)
    {
        GetBreedsQuery query = new(speciesId);
        return await _mediator.Send(query);
    }

    [HttpGet]
    public async Task<IList<DropdownDataResponse<string>>> GetBreedsByName(string breedName)
    {
        GetBreedsByNameQuery query = new(breedName);
        return await _mediator.Send(query);
    }
}