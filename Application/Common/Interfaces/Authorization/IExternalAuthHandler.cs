using Application.Commands.Users.ExternalLogin;

namespace Application.Common.Interfaces.Authorization;

public interface IExternalAuthHandler
{
    Task<ExternalAuthPayload?> ValidateGoogleToken(ExternalLoginCommand externalAuth);
    Task<ExternalAuthPayload?> ValidateFacebookToken(ExternalLoginCommand externalAuth);
}