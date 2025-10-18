using Application.Common.Interfaces.Persistence;
using Application.Queries.Users.Common;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.Users.GetById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDataResponse>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDataResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(IAppDbContext dbContext, ILogger<GetUserByIdQueryHandler> logger)
    {
        _logger = Guard.Against.Null(logger);
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<UserDataResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        UserDataResponse? searchedUser = await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == request.UserId)
            .Select(user => new UserDataResponse(
                    user.Id,
                    user.Image,
                    user.FullName,
                    user.Email!,
                    user.PhoneNumber,
                    user.ReceivesOnlyWhatsAppMessages,
                    user.DefaultAdoptionFormUrl
                )
            ).SingleOrDefaultAsync(cancellationToken);
        if (searchedUser is null)
        {
            _logger.LogInformation("Não foi possível obter usuário {UserId}", request.UserId);
            throw new NotFoundException("Não foi possível obter o usuário com o id especificado.");
        }

        return searchedUser;
    }
}