using Application.Common.DTOs;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class FoundAnimalFavoriteMappings
{
	extension(FoundAnimalFavorite adoptionFavorite)
	{
		public FoundAnimalFavoriteResponse ToFoundAnimalFavoriteResponse()
		{
			return new FoundAnimalFavoriteResponse(
				Id: adoptionFavorite.Id,
				FoundAlert: adoptionFavorite.FoundAnimalAlert.ToSimplifiedFoundAlertResponse()
			);
		}
	}
}