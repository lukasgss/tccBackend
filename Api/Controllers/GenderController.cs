using Application.Common.Interfaces.FrontendDropdownData;
using Application.Queries.Genders.GetGenders;
using Application.Queries.Genders.GetGendersForFilters;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/genders")]
public class GenderController : ControllerBase
{
    private readonly ISender _mediator;

    public GenderController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("dropdown")]
    public async Task<IList<DropdownDataResponse<string>>> ListAges()
    {
        return await _mediator.Send(new GetGendersQuery());
    }

    [HttpGet("filters")]
    public async Task<IList<DropdownDataResponse<string>>> ListAgesForFilters()
    {
        return await _mediator.Send(new GetGendersForFiltersQuery());
    }
}