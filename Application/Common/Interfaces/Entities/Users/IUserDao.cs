namespace Application.Common.Interfaces.Entities.Users;

public interface IUserDao
{
    Task<bool> UserWithEmailExists(string email);
}