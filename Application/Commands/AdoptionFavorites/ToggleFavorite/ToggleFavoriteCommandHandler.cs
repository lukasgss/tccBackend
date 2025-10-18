using Application.Common.DTOs;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;
using MediatR;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionFavorites.ToggleFavorite;

public record ToggleFavoriteCommand(Guid UserId, Guid AlertId) : IRequest<AdoptionFavoriteResponse>;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, AdoptionFavoriteResponse>
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly IAdoptionFavoritesRepository _adoptionFavoritesRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValueProvider _valueProvider;

    public ToggleFavoriteCommandHandler(
        IAdoptionAlertRepository adoptionAlertRepository,
        IAdoptionFavoritesRepository adoptionFavoritesRepository,
        IUserRepository userRepository,
        IValueProvider valueProvider)
    {
        _valueProvider = Guard.Against.Null(valueProvider);
        _userRepository = Guard.Against.Null(userRepository);
        _adoptionFavoritesRepository = Guard.Against.Null(adoptionFavoritesRepository);
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
    }

    public async Task<AdoptionFavoriteResponse> Handle(ToggleFavoriteCommand request,
        CancellationToken cancellationToken)
    {
        AdoptionAlert? adoptionAlert = await _adoptionAlertRepository.GetByIdAsync(request.AlertId);
        if (adoptionAlert is null)
        {
            throw new NotFoundException("Alerta com o id especificado não existe.");
        }

        User user = (await _userRepository.GetUserByIdAsync(request.UserId))!;

        AdoptionFavorite? adoptionFavorite =
            await _adoptionFavoritesRepository.GetFavoriteAlertAsync(request.UserId, request.AlertId);
        if (adoptionFavorite is null)
        {
            AdoptionFavorite favorite = new()
            {
                Id = _valueProvider.NewGuid(),
                User = user,
                UserId = user.Id,
                AdoptionAlert = adoptionAlert,
                AdoptionAlertId = adoptionAlert.Id
            };
            _adoptionFavoritesRepository.Add(favorite);
            await _adoptionFavoritesRepository.CommitAsync();

            return favorite.ToAdoptionFavoriteResponse();
        }

        _adoptionFavoritesRepository.Delete(adoptionFavorite);
        await _adoptionFavoritesRepository.CommitAsync();

        return adoptionFavorite.ToAdoptionFavoriteResponse();
    }
}