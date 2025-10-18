using Application.Common.Exceptions;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Breeds.GetBreeds;

public record GetBreedsByNameQuery(string Name) : IRequest<IList<DropdownDataResponse<string>>>;

public class GetBreedsByNameQueryHandler : IRequestHandler<GetBreedsByNameQuery, IList<DropdownDataResponse<string>>>
{
    private readonly IAppDbContext _dbContext;

    public GetBreedsByNameQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<DropdownDataResponse<string>>> Handle(GetBreedsByNameQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Name.Length < 2)
        {
            throw new BadRequestException("Preencha no mínimo 2 caracteres para buscar a raça pelo nome.");
        }

        return await _dbContext.Breeds
            .AsNoTracking()
            .Where(breed => breed.Name.ToLower().Contains(request.Name.ToLowerInvariant()))
            .Select(breed => new DropdownDataResponse<string>()
            {
                Label = breed.Name,
                Value = breed.Id.ToString()
            })
            .ToListAsync(cancellationToken);
    }
}