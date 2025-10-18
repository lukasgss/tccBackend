using Application.Common.Interfaces.FrontendDropdownData;
using Application.Queries.Ages.GetAges;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/ages")]
public class AgeController : ControllerBase
{
    private readonly ISender _mediator;

    public AgeController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("dropdown")]
    public async Task<IList<DropdownDataResponse<string>>> ListAges()
    {
        return await _mediator.Send(new GetAgesQuery());
    }
}