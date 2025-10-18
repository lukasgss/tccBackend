using Application.Commands.Users.Images.Upload;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Application.Queries.Users.Common;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.Users.Edit;

public record EditUserCommand(
    Guid LoggedInUserId,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    bool OnlyWhatsAppMessages,
    IFormFile? Image,
    string ExistingImage,
    IFormFile? DefaultAdoptionForm
) : IRequest<UserDataResponse>;

public class EditUserCommandHandler : IRequestHandler<EditUserCommand, UserDataResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ISender _mediator;
    private readonly IAdoptionAlertFileSubmissionService _adoptionAlertFileSubmissionService;
    private readonly ILogger<EditUserCommandHandler> _logger;

    public EditUserCommandHandler(
        IUserRepository userRepository,
        ISender mediator,
        ILogger<EditUserCommandHandler> logger,
        IAdoptionAlertFileSubmissionService adoptionAlertFileSubmissionService)
    {
        _adoptionAlertFileSubmissionService = Guard.Against.Null(adoptionAlertFileSubmissionService);
        _mediator = Guard.Against.Null(mediator);
        _userRepository = Guard.Against.Null(userRepository);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<UserDataResponse> Handle(EditUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId != request.LoggedInUserId)
        {
            _logger.LogInformation("Usuário {UserId} não possui permissão para editar {OtherUserId}",
                request.LoggedInUserId, request.UserId);
            throw new ForbiddenException("Você não possui permissão para editar este usuário.");
        }

        User? user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            _logger.LogInformation("Usuário {UserId} não existe", request.UserId);
            throw new NotFoundException("Usuário com o id especificado não existe.");
        }

        string userImageUrl;
        if (request.Image is not null)
        {
            UploadUserImageCommand uploadImageCommand = new(request.Image, user.Image);
            userImageUrl = await _mediator.Send(uploadImageCommand, cancellationToken);
        }
        else
        {
            userImageUrl = request.ExistingImage;
        }

        string? adoptionFormUrl = null;

        if (request.DefaultAdoptionForm is not null)
        {
            adoptionFormUrl =
                await _adoptionAlertFileSubmissionService.UploadUserDefaultAdoptionFormAsync(
                    request.DefaultAdoptionForm,
                    user.DefaultAdoptionFormUrl);
        }

        user.FullName = request.FullName;
        // TODO: Later on, add some kind of phone validation with SMS
        user.PhoneNumber = request.PhoneNumber;
        user.Image = userImageUrl;
        user.ReceivesOnlyWhatsAppMessages = request.OnlyWhatsAppMessages;
        user.DefaultAdoptionFormUrl = adoptionFormUrl;

        await _userRepository.CommitAsync();

        return user.ToUserDataResponse();
    }
}