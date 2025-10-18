using Application.Queries.Colors.GetAll;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/colors")]
public class ColorController : ControllerBase
{
    private readonly ISender _mediator;

    public ColorController(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("dropdown")]
    public async Task<IList<ColorResponse>> GetAllColors()
    {
        return await _mediator.Send(new GetAllColorsQuery());
    }
}