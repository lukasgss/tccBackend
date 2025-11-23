using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.FoundAnimalFavoriteAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;
using MediatR;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.FoundAlertFavorites.ToggleFavorite;

[ExcludeFromCodeCoverage]
public sealed record ToggleFoundAlertFavoriteCommand(Guid UserId, Guid AlertId) : IRequest<FoundAnimalFavoriteResponse>;

public sealed class ToggleFoundAlertFavoriteCommandHandler : IRequestHandler<ToggleFoundAlertFavoriteCommand,
	FoundAnimalFavoriteResponse>
{
	private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepository;
	private readonly IFoundAnimalFavoritesRepository _foundAnimalFavoritesRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;

	public ToggleFoundAlertFavoriteCommandHandler(
		IFoundAnimalAlertRepository foundAnimalAlertRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider,
		IFoundAnimalFavoritesRepository foundAnimalFavoritesRepository)
	{
		_foundAnimalFavoritesRepository = Guard.Against.Null(foundAnimalFavoritesRepository);
		_valueProvider = Guard.Against.Null(valueProvider);
		_userRepository = Guard.Against.Null(userRepository);
		_foundAnimalAlertRepository = Guard.Against.Null(foundAnimalAlertRepository);
	}

	public async Task<FoundAnimalFavoriteResponse> Handle(ToggleFoundAlertFavoriteCommand request,
		CancellationToken cancellationToken)
	{
		FoundAnimalAlert? foundAnimalAlert = await _foundAnimalAlertRepository.GetByIdAsync(request.AlertId);
		if (foundAnimalAlert is null)
		{
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		User user = (await _userRepository.GetUserByIdAsync(request.UserId))!;

		FoundAnimalFavorite? foundFavorite =
			await _foundAnimalFavoritesRepository.GetFavoriteAlertAsync(request.UserId, request.AlertId);
		if (foundFavorite is null)
		{
			FoundAnimalFavorite favorite = new()
			{
				Id = _valueProvider.NewGuid(),
				User = user,
				UserId = user.Id,
				FoundAnimalAlert = foundAnimalAlert,
				FoundAnimalAlertId = foundAnimalAlert.Id
			};
			_foundAnimalFavoritesRepository.Add(favorite);
			await _foundAnimalFavoritesRepository.CommitAsync();

			return favorite.ToFoundAnimalFavoriteResponse();
		}

		_foundAnimalFavoritesRepository.Delete(foundFavorite);
		await _foundAnimalFavoritesRepository.CommitAsync();

		return foundFavorite.ToFoundAnimalFavoriteResponse();
	}
}