using Application.Common.DTOs;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Pagination;
using Application.Queries.MissingAlerts.ListMissingAlerts;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping;

public static class PagedListMappings
{
    extension(PagedList<UserMessage> pagedUserMessages)
    {
        public PaginatedEntity<UserMessageResponse> ToUserMessageResponsePagedList()
        {
            List<UserMessageResponse> userMessageResponses = pagedUserMessages
                .Select(message => message.ToUserMessageResponse())
                .ToList();

            return new PaginatedEntity<UserMessageResponse>
            {
                Data = userMessageResponses,
                CurrentPage = pagedUserMessages.CurrentPage,
                CurrentPageCount = pagedUserMessages.CurrentPageCount,
                TotalPages = pagedUserMessages.TotalPages
            };
        }
    }

    extension(PagedList<AdoptionAlertListing> adoptionAlerts)
    {
        public PaginatedEntity<AdoptionAlertListingResponse> ToAdoptionAlertListingResponsePagedList()
        {
            List<AdoptionAlertListingResponse> adoptionAlertResponses = adoptionAlerts
                .Select(alert => alert.ToAdoptionAlertListingResponse())
                .ToList();

            return new PaginatedEntity<AdoptionAlertListingResponse>
            {
                Data = adoptionAlertResponses,
                CurrentPage = adoptionAlerts.CurrentPage,
                CurrentPageCount = adoptionAlerts.CurrentPageCount,
                TotalPages = adoptionAlerts.TotalPages
            };
        }
    }

    extension(PagedList<MissingAlertQueryResponse> missingAlerts)
    {
        public PaginatedEntity<MissingAlertResponse> ToMissingAlertResponsePagedList()
        {
            List<MissingAlertResponse> missingAlertResponses = missingAlerts
                .Select(alert => alert.ToMissingAlertResponseFromQuery())
                .ToList();

            return new PaginatedEntity<MissingAlertResponse>
            {
                Data = missingAlertResponses,
                CurrentPage = missingAlerts.CurrentPage,
                CurrentPageCount = missingAlerts.CurrentPageCount,
                TotalPages = missingAlerts.TotalPages
            };
        }
    }

    extension(PagedList<FoundAnimalAlert> foundAnimalAlerts)
    {
        public PaginatedEntity<FoundAnimalAlertResponse> ToFoundAnimalAlertResponsePagedList()
        {
            List<FoundAnimalAlertResponse> missingAlertResponses = foundAnimalAlerts
                .Select(alert => alert.ToFoundAnimalAlertResponse())
                .ToList();

            return new PaginatedEntity<FoundAnimalAlertResponse>
            {
                Data = missingAlertResponses,
                CurrentPage = foundAnimalAlerts.CurrentPage,
                CurrentPageCount = foundAnimalAlerts.CurrentPageCount,
                TotalPages = foundAnimalAlerts.TotalPages
            };
        }
    }

    extension(PagedList<AdoptionFavoriteResponse> adoptionFavorites)
    {
        public PaginatedEntity<AdoptionFavoriteResponse> ToAlertFavoritesResponse()
        {
            return new PaginatedEntity<AdoptionFavoriteResponse>
            {
                Data = adoptionFavorites,
                CurrentPage = adoptionFavorites.CurrentPage,
                CurrentPageCount = adoptionFavorites.CurrentPageCount,
                TotalPages = adoptionFavorites.TotalPages
            };
        }
    }
}