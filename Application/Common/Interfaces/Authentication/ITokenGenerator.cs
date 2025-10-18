using Application.Common.DTOs;

namespace Application.Common.Interfaces.Authentication;

public interface ITokenGenerator
{
    TokensResponse GenerateTokens(Guid userId, string fullName);
}