using Application.Common.Interfaces.FrontendDropdownData;
using Application.Queries.Sizes.GetSizes;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/sizes")]
public class SizeController : ControllerBase
{
    private readonly ISender _mediator;

    public SizeController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("dropdown")]
    public async Task<IList<DropdownDataResponse<string>>> ListSizes()
    {
        return await _mediator.Send(new GetAllSizesQuery());
    }
}