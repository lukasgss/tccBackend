using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Entities.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<SignInResult> CheckCredentials(User user, string password);
    Task<IdentityResult> RegisterUserAsync(User user);
    Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo userLoginInfo);
    Task<IdentityResult> RegisterUserAsync(User user, string password);
    Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled);
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<IdentityResult> ResetPasswordAsync(User user, string resetToken, string newPassword);
    Task<User?> FindByLoginAsync(string loginProvider, string providerKey);
    Task<User?> FindByEmailAsync(string email);
    Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsLockedOutAsync(User user);
    Task<bool> IsEmailConfirmedAsync(User user);
}