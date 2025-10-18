using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserRepository(
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public async Task<SignInResult> CheckCredentials(User user, string password)
    {
        return await _signInManager.CheckPasswordSignInAsync(
            user: user,
            password: password,
            lockoutOnFailure: true);
    }

    public async Task<IdentityResult> RegisterUserAsync(User user)
    {
        return await _userManager.CreateAsync(user);
    }

    public async Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo userLoginInfo)
    {
        return await _userManager.AddLoginAsync(user, userLoginInfo);
    }

    public async Task<IdentityResult> RegisterUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled)
    {
        return await _userManager.SetLockoutEnabledAsync(user, enabled);
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<IdentityResult> ResetPasswordAsync(User user, string resetToken, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
    }

    public async Task<User?> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return await _userManager.FindByLoginAsync(loginProvider, providerKey);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<List<User>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        return await _dbContext.Users.Where(user => userIds.Contains(user.Id))
            .ToListAsync();
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> IsLockedOutAsync(User user)
    {
        return await _userManager.IsLockedOutAsync(user);
    }

    public async Task<bool> IsEmailConfirmedAsync(User user)
    {
        return await _userManager.IsEmailConfirmedAsync(user);
    }
}