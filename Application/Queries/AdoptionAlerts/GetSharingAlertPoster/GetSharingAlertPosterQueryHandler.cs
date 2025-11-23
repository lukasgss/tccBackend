using System.Diagnostics.CodeAnalysis;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.SharingAlerts;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.AdoptionAlerts.GetSharingAlertPoster;

[ExcludeFromCodeCoverage]
public sealed record GetSharingAlertPosterQuery(Guid AlertId) : IRequest<SharingAlertPosterResponse>;

public sealed class GetSharingAlertPosterQueryHandler
    : IRequestHandler<GetSharingAlertPosterQuery, SharingAlertPosterResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ISharingPosterGenerator _sharingPosterGenerator;

    public GetSharingAlertPosterQueryHandler(IAppDbContext dbContext, ISharingPosterGenerator sharingPosterGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext);
        _sharingPosterGenerator = Guard.Against.Null(sharingPosterGenerator);
    }

    public async Task<SharingAlertPosterResponse> Handle(GetSharingAlertPosterQuery request,
        CancellationToken cancellationToken)
    {
        AdoptionAlertPosterData? adoptionAlertData = await _dbContext
            .AdoptionAlerts
            .AsNoTracking()
            .Where(a => a.Id == request.AlertId)
            .Select(a => new AdoptionAlertPosterData(
                a.Pet.Name,
                a.Pet.Images.First().ImageUrl,
                a.Pet.Species.ToSpeciesResponse(),
                a.Pet.Breed.ToBreedResponse(),
                a.Pet.Gender.ToGenderResponse(),
                a.Pet.IsCastrated,
                a.Pet.IsNegativeToFivFelv,
                a.Pet.Age.ToAgeResponse(),
                a.Pet.Size.ToSizeResponse(),
                a.Pet.Colors.ToListOfColorResponse(),
                a.Pet.IsVaccinated,
                a.Description,
                a.AdoptionRestrictions,
                a.User.FullName,
                a.User.PhoneNumber!
            ))
            .SingleOrDefaultAsync(cancellationToken);
        if (adoptionAlertData is null)
        {
            throw new NotFoundException("Não foi possível encontrar o alerta de adoção especificado.");
        }

        var stream = await _sharingPosterGenerator.GenerateAdoptionPoster(adoptionAlertData);

        return new SharingAlertPosterResponse(stream, adoptionAlertData.PetName);
    }
}