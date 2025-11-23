using System.Diagnostics.CodeAnalysis;
using Application.Common.Calculators;
using Application.Common.Converters;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Persistence;
using Application.Common.Pagination;
using Ardalis.GuardClauses;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.AdoptionAlerts.ListAlerts;

[ExcludeFromCodeCoverage]
public record ListAdoptionAlertsQuery(
    AdoptionAlertFilters Filters,
    Guid? UserId,
    int Page = 1,
    int PageSize = 30) : IRequest<PaginatedEntity<AdoptionAlertListingResponse>>;

public class ListAdoptionAlertsQueryHandler
    : IRequestHandler<ListAdoptionAlertsQuery, PaginatedEntity<AdoptionAlertListingResponse>>
{
    private readonly IAppDbContext _dbContext;

    public ListAdoptionAlertsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<PaginatedEntity<AdoptionAlertListingResponse>> Handle(ListAdoptionAlertsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1 || request.PageSize < 1)
        {
            throw new BadRequestException("Insira um número e tamanho de página maior ou igual a 1.");
        }

        var filteredAlerts = await ListAdoptionAlerts(request);

        return filteredAlerts.ToAdoptionAlertListingResponsePagedList();
    }

    private async Task<PagedList<AdoptionAlertListing>> ListAdoptionAlerts(ListAdoptionAlertsQuery request)
    {
        var query = _dbContext.AdoptionAlerts
            .AsSplitQuery()
            .AsNoTracking()
            .Include(alert => alert.Pet)
            .Include(alert => alert.Pet.Images)
            .Include(alert => alert.Pet.Colors)
            .Include(alert => alert.Pet.Breed)
            .Include(alert => alert.Pet.Species)
            .Include(alert => alert.User)
            .Include(alert => alert.AdoptionFavorites)
            .ThenInclude(favorite => favorite.User)
            .Where(alert =>
                (request.Filters.NotAdopted && alert.AdoptionDate == null) ||
                (request.Filters.Adopted && alert.AdoptionDate != null) ||
                (request.Filters.NotAdopted && request.Filters.Adopted))
            .Select(alert => new AdoptionAlertListing()
            {
                Id = alert.Id,
                Location = alert.Location,
                Description = alert.Description,
                RegistrationDate = alert.RegistrationDate,
                AdoptionDate = alert.AdoptionDate,
                AdoptionRestrictions = alert.AdoptionRestrictions,
                Pet = alert.Pet,
                User = alert.User,
                City = alert.City,
                IsFavorite = alert.AdoptionFavorites.Any(favorite => favorite.User.Id == request.UserId)
            });

        query = ApplyFilters(query, request.Filters);

        return await query.ToPaginatedListAsync(request.Page, request.PageSize);
    }

    private static IQueryable<AdoptionAlertListing> ApplyFilters(IQueryable<AdoptionAlertListing> query,
        AdoptionAlertFilters filters)
    {
        if (filters.HasGeoFilters())
        {
            var filtersLocation =
                CoordinatesCalculator.CreatePointBasedOnCoordinates(filters.Latitude!.Value, filters.Longitude!.Value);
            var filteredDistanceInMeters = UnitsConverter.ConvertKmToMeters(filters.RadiusDistanceInKm!.Value);

            query = query.Where(alert => alert.Location != null &&
                                         EF.Functions.IsWithinDistance(alert.Location, filtersLocation, filteredDistanceInMeters, true));
        }

        if (filters.City is not null)
        {
            query = query.Where(alert =>
                EF.Functions.Unaccent(alert.City.Name.ToLower()) == EF.Functions.Unaccent(filters.City.ToLower()));
        }

        if (filters.BreedIds is not null)
        {
            query = query.Where(alert => filters.BreedIds.Contains(alert.Pet.Breed.Id));
        }

        if (filters.GenderIds is not null)
        {
            query = query.Where(alert => filters.GenderIds.Contains(alert.Pet.Gender) ||
                                         alert.Pet.Gender == Gender.Desconhecido);
        }

        if (filters.SpeciesId is not null)
        {
            query = query.Where(alert => alert.Pet.Species.Id == filters.SpeciesId);
        }

        if (filters.ColorIds is not null)
        {
            query = query.Where(alert => alert.Pet.Colors.Any(color => filters.ColorIds.Contains(color.Id)));
        }

        if (filters.AgeIds is not null)
        {
            query = query.Where(alert => filters.AgeIds.Contains(alert.Pet.Age));
        }

        if (filters.SizeIds is not null)
        {
            query = query.Where(alert => filters.SizeIds.Contains(alert.Pet.Size));
        }

        return query;
    }
}