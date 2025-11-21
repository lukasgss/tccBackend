using Application.Commands.AdoptionAlerts.ReportAdoptionAlert;
using Application.Commands.MissingAlerts.ReportMissingAlert;
using Application.Common.Interfaces.Authorization;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/missing-reports")]
public sealed class MissingReportsController : ControllerBase
{
	private readonly ISender _mediator;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public MissingReportsController(ISender mediator, IUserAuthorizationService userAuthorizationService)
	{
		_userAuthorizationService = Guard.Against.Null(userAuthorizationService);
		_mediator = Guard.Against.Null(mediator);
	}

	[HttpPost("{alertId:guid}")]
	public async Task<ActionResult> ReportAsync(Guid alertId, CreateAdoptionReport createAdoptionReport)
	{
		Guid? userId = _userAuthorizationService.GetPotentialUserId(User);

		ReportMissingAlertCommand command = new(alertId, createAdoptionReport.Reason, userId);
		await _mediator.Send(command);

		return NoContent();
	}
}