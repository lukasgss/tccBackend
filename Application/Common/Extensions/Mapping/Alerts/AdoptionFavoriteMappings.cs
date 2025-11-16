using Application.Common.DTOs;
using Domain.Entities.Alerts.UserFavorites;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class AdoptionFavoriteMappings
{
    extension(AdoptionFavorite adoptionFavorite)
    {
        public AdoptionFavoriteResponse ToAdoptionFavoriteResponse()
        {
            return new AdoptionFavoriteResponse(
                Id: adoptionFavorite.Id,
                AdoptionAlert: adoptionFavorite.AdoptionAlert.ToSimplifiedAdoptionAlertResponse()
            );
        }
    }
}