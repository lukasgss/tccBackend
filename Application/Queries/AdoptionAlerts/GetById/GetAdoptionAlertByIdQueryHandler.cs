using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Location;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Persistence;
using Application.Queries.AdoptionFavorites.GetById;
using Application.Queries.Users.Common;
using Ardalis.GuardClauses;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.AdoptionAlerts.GetById;

public record GetAdoptionAlertByIdQuery(Guid AlertId, Guid? UserId) : IRequest<AdoptionAlertResponseWithGeoLocation>;

public record AdoptionAlertByIdQueryResponse(
    Guid Id,
    List<string> AdoptionRestrictions,
    Point? Location,
    string? Description,
    DateTime RegistrationDate,
    DateOnly? AdoptionDate,
    string Neighborhood,
    State State,
    City City,
    PetResponseNoOwner Pet,
    AlertUserDataResponse User,
    FileAttachment? AdoptionForm
);

public class GetAdoptionAlertByIdQueryHandler
    : IRequestHandler<GetAdoptionAlertByIdQuery, AdoptionAlertResponseWithGeoLocation>
{
    private readonly IAppDbContext _dbContext;

    public GetAdoptionAlertByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<AdoptionAlertResponseWithGeoLocation> Handle(GetAdoptionAlertByIdQuery request,
        CancellationToken cancellationToken)
    {
        var adoptionAlert = await _dbContext.AdoptionAlerts
                                .AsNoTracking()
                                .Include(alert => alert.Pet)
                                .Include(alert => alert.Pet.Images)
                                .Include(alert => alert.Pet.Colors)
                                .Include(alert => alert.Pet.Breed)
                                .Include(alert => alert.Pet.Species)
                                .Include(alert => alert.User)
                                .Include(alert => alert.State)
                                .Include(alert => alert.City)
                                .Where(alert => alert.Id == request.AlertId)
                                .Select(alert => new AdoptionAlertByIdQueryResponse(
                                    alert.Id,
                                    alert.AdoptionRestrictions,
                                    alert.Location,
                                    alert.Description,
                                    alert.RegistrationDate,
                                    alert.AdoptionDate,
                                    alert.Neighborhood,
                                    alert.State,
                                    alert.City,
                                    alert.Pet.ToPetResponseNoOwner(),
                                    new AlertUserDataResponse(
                                        alert.User.Id,
                                        alert.User.Image,
                                        alert.User.FullName,
                                        alert.User.PhoneNumber,
                                        alert.User.ReceivesOnlyWhatsAppMessages
                                    ),
                                    alert.AdoptionForm
                                ))
                                .FirstOrDefaultAsync(cancellationToken)
                            ?? throw new NotFoundException("Alerta de adoção com o id especificado não existe.");

        bool isFavorite = false;
        if (request.UserId is not null)
        {
            isFavorite = await _dbContext.AdoptionFavorites
                .AnyAsync(favorite => favorite.AdoptionAlertId == request.AlertId && favorite.UserId == request.UserId,
                    cancellationToken: cancellationToken);
        }

        AlertGeoLocation formattedLocation = new(
            City: new LocationResponse(adoptionAlert.City.Id, adoptionAlert.City.Name),
            Neighborhood: adoptionAlert.Neighborhood,
            State: new LocationResponse(adoptionAlert.State.Id, adoptionAlert.State.Name)
        );

        return adoptionAlert.ToAdoptionAlertResponseWithGeoLocation(formattedLocation, isFavorite);
    }
}