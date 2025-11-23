using System.Diagnostics.CodeAnalysis;
using Application.Common.Calculators;
using Application.Common.Converters;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Persistence;
using Application.Common.Pagination;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Application.Queries.MissingAlerts.ListMissingAlerts;

[ExcludeFromCodeCoverage]
public record ListMissingAlertsQuery(MissingAlertFilters Filters, int Page, int PageSize)
    : IRequest<PaginatedEntity<MissingAlertResponse>>;

public sealed class ListMissingAlertsQueryHandler
    : IRequestHandler<ListMissingAlertsQuery, PaginatedEntity<MissingAlertResponse>>
{
    private readonly IAppDbContext _dbContext;

    public ListMissingAlertsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<PaginatedEntity<MissingAlertResponse>> Handle(ListMissingAlertsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1 || request.PageSize < 1)
        {
            throw new BadRequestException("Insira um número e tamanho de página maior ou igual a 1.");
        }

        var query = _dbContext.MissingAlerts
            .AsSplitQuery()
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Species)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Images)
            // filters records based if it should show only missing alerts
            // (RecoveryDate != null), show only non recovered alerts
            // (RecoveryDate == null) or both, if both filters.Missing
            // or filters.NotMissing are true
            .Where(alert => alert.RecoveryDate == null == request.Filters.Missing ||
                            alert.RecoveryDate != null == request.Filters.NotMissing);

        query = ApplyFilters(query, request.Filters);

        var projectedQuery = query
            .Select(alert => new MissingAlertQueryResponse(
                    alert.Id,
                    alert.RegistrationDate,
                    alert.State,
                    alert.City,
                    alert.Neighborhood,
                    alert.Description,
                    alert.RecoveryDate,
                    alert.Pet.ToPetResponseNoOwner(),
                    alert.User.ToOwnerResponse()
                )
            );

        var filteredAlerts =
            await PagedList<MissingAlertQueryResponse>.ToPagedListAsync(projectedQuery, request.Page, request.PageSize);

        return filteredAlerts.ToMissingAlertResponsePagedList();
    }

    private static IQueryable<MissingAlert> ApplyFilters(IQueryable<MissingAlert> query,
        MissingAlertFilters filters)
    {
        if (filters.HasGeoFilters())
        {
            Point filterLocation =
                CoordinatesCalculator.CreatePointBasedOnCoordinates(filters.Latitude!.Value, filters.Longitude!.Value);
            double filteredDistanceInMeters = UnitsConverter.ConvertKmToMeters(filters.RadiusDistanceInKm!.Value);

            query = query.Where(alert => alert.Location.Distance(filterLocation) <= filteredDistanceInMeters);
        }

        if (filters.BreedIds is not null)
        {
            query = query.Where(alert => filters.BreedIds.Contains(alert.Pet.Breed.Id));
        }

        if (filters.GenderIds is not null)
        {
            query = query.Where(alert => filters.GenderIds.Contains(alert.Pet.Gender));
        }

        if (filters.SpeciesId is not null)
        {
            query = query.Where(alert => alert.Pet.Species.Id == filters.SpeciesId);
        }

        if (filters.ColorIds is not null)
        {
            query = query.Where(alert => alert.Pet.Colors.Any(color => filters.ColorIds.Contains(color.Id)));
        }

        return query;
    }
}