using Application.Commands.AdoptionAlerts.ReportAdoptionAlert;
using Application.Common.Interfaces.Authorization;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/adoption-reports")]
public class AdoptionReportsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public AdoptionReportsController(ISender mediator, IUserAuthorizationService userAuthorizationService)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpPost("{alertId:guid}")]
    public async Task<ActionResult> Report(Guid alertId, CreateAdoptionReport createAdoptionReport)
    {
        Guid? userId = _userAuthorizationService.GetPotentialUserId(User);

        ReportAdoptionAlertCommand command = new(alertId, createAdoptionReport.Reason, userId);
        await _mediator.Send(command);

        return NoContent();
    }
}