using Application.Commands.Users.ChangePassword;
using Application.Commands.Users.ConfirmEmail;
using Application.Commands.Users.Edit;
using Application.Commands.Users.ExternalLogin;
using Application.Commands.Users.ForgotPassword;
using Application.Commands.Users.Login;
using Application.Commands.Users.RedefinePassword;
using Application.Commands.Users.RefreshToken;
using Application.Commands.Users.Register;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Queries.Users.Common;
using Application.Queries.Users.GetById;
using Application.Queries.Users.GetProfile;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public UserController(IUserAuthorizationService userAuthorizationService, ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<UserDataResponse> GetUserById(Guid userId)
    {
        return await _mediator.Send(new GetUserByIdQuery(userId));
    }

    [HttpGet("profile/{userId:guid}")]
    public async Task<UserProfileResponse> GetProfile(Guid userId)
    {
        return await _mediator.Send(new GetProfileQuery(userId));
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterUserCommand command)
    {
        UserResponse createdUser = await _mediator.Send(command);

        return new CreatedAtRouteResult(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
    }

    [HttpPost("login")]
    public async Task<UserResponse> Login(LoginCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPost("external-login")]
    public async Task<UserResponse> ExternalLogin(ExternalLoginCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(ConfirmEmailCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("reset-password-forgot")]
    public async Task<ActionResult> RedefinePassword(RedefinePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // This endpoint is meant to be accessed with the refresh token instead of the access token
    [Authorize]
    [HttpPost("refresh")]
    public async Task<TokensResponse> Refresh()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _mediator.Send(new RefreshTokenCommand(userId));
    }

    [Authorize]
    [HttpPut("password")]
    public async Task<ActionResult> ChangeUserPassword(ChangePasswordRequest request)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        ChangePasswordCommand command = new(
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmNewPassword,
            userId);
        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize]
    [HttpPut("{userRouteId:guid}")]
    public async Task<UserDataResponse> Update([FromForm] EditUserRequest editUserRequest, Guid userRouteId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        EditUserCommand command = new(
            userId,
            userRouteId,
            editUserRequest.FullName,
            editUserRequest.PhoneNumber,
            editUserRequest.OnlyWhatsAppMessages,
            editUserRequest.Image,
            editUserRequest.ExistingImage,
            editUserRequest.DefaultAdoptionForm);

        return await _mediator.Send(command);
    }
}