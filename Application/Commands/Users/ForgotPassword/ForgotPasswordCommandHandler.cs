using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Messaging;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Commands.Users.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IMessagingService _messagingService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IMessagingService messagingService)
    {
        _messagingService = Guard.Against.Null(messagingService);
        _userRepository = Guard.Against.Null(userRepository);
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return;
        }

        string token = await _userRepository.GeneratePasswordResetTokenAsync(user);

        await _messagingService.SendForgotPasswordMessageAsync(user.Email!, user.FullName, token);
    }
}