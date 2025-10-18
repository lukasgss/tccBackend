using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.DataAccessObjects;

public class UserDao : IUserDao
{
    private readonly IAppDbContext _dbContext;

    public UserDao(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<bool> UserWithEmailExists(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }
}