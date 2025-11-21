using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Location;
using Application.Common.Interfaces.Persistence;
using Application.Queries.Users.Common;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.MissingAlerts.GetById;

public record GetMissingAlertByIdQuery(Guid AlertId) : IRequest<MissingAlertResponseWithGeoLocation>;

public sealed class GetMissingAlertByIdQueryHandler
    : IRequestHandler<GetMissingAlertByIdQuery, MissingAlertResponseWithGeoLocation>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetMissingAlertByIdQueryHandler> _logger;

    public GetMissingAlertByIdQueryHandler(IAppDbContext dbContext, ILogger<GetMissingAlertByIdQueryHandler> logger)
    {
        _logger = Guard.Against.Null(logger);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<MissingAlertResponseWithGeoLocation> Handle(GetMissingAlertByIdQuery request,
        CancellationToken cancellationToken)
    {
        var missingAlert = await _dbContext.MissingAlerts
            .AsNoTracking()
            .AsSplitQuery()
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Species)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Images)
            .Where(alert => alert.Id == request.AlertId)
            .Select(alert => new MissingAlertByIdQueryResponse(
                alert.Id,
                alert.RegistrationDate,
                alert.Description,
                alert.RecoveryDate,
                alert.Location.Y,
                alert.Location.X,
                alert.City,
                alert.State,
                alert.Neighborhood,
                alert.Pet.ToPetResponseNoOwner(),
                new AlertUserDataResponse(alert.User.Id, alert.User.Image, alert.User.FullName, alert.User.PhoneNumber!,
                    alert.User.ReceivesOnlyWhatsAppMessages)
            ))
            .FirstOrDefaultAsync(cancellationToken);
        if (missingAlert is null)
        {
            _logger.LogInformation("Alerta {MissingAlertId} não existe", request.AlertId);
            throw new NotFoundException("Alerta com o id especificado não existe.");
        }

        AlertGeoLocation formattedLocation = new(
            City: new LocationResponse(missingAlert.City.Id, missingAlert.City.Name),
            Neighborhood: missingAlert.Neighborhood,
            State: new LocationResponse(missingAlert.State.Id, missingAlert.State.Name)
        );

        return missingAlert.ToMissingAlertResponseWithGeoLocation(formattedLocation);
    }
}