using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.MissingAlerts.GetById;

public record GetMissingAlertByIdQuery(Guid AlertId) : IRequest<MissingAlertResponse>;

public class GetMissingAlertByIdQueryHandler : IRequestHandler<GetMissingAlertByIdQuery, MissingAlertResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetMissingAlertByIdQueryHandler> _logger;

    public GetMissingAlertByIdQueryHandler(IAppDbContext dbContext, ILogger<GetMissingAlertByIdQueryHandler> logger)
    {
        _logger = Guard.Against.Null(logger);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<MissingAlertResponse> Handle(GetMissingAlertByIdQuery request,
        CancellationToken cancellationToken)
    {
        MissingAlertResponse? missingAlert = await _dbContext.MissingAlerts
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Species)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Breed)
            .Include(alert => alert.Pet)
            .ThenInclude(pet => pet.Images)
            .Select(alert => new MissingAlertResponse(
                    alert.Id,
                    alert.RegistrationDate,
                    alert.Location.Y,
                    alert.Location.X,
                    alert.Description,
                    alert.RecoveryDate,
                    alert.Pet.ToPetResponseNoOwner(),
                    alert.User.ToOwnerResponse()
                )
            )
            .SingleOrDefaultAsync(alert => alert.Id == request.AlertId, cancellationToken);
        if (missingAlert is null)
        {
            _logger.LogInformation("Alerta {MissingAlertId} não existe", request.AlertId);
            throw new NotFoundException("Alerta com o id especificado não existe.");
        }

        return missingAlert;
    }
}