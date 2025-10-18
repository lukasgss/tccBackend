using Application.Common.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMappings
{
    public static OwnerResponse ToOwnerResponse(this User user)
    {
        return new OwnerResponse(
            FullName: user.FullName,
            Image: user.Image,
            PhoneNumber: user.PhoneNumber!,
            Email: user.Email!
        );
    }

    public static UserDataResponse ToUserDataResponse(this User user)
    {
        return new UserDataResponse(
            Id: user.Id,
            FullName: user.FullName,
            Image: user.Image,
            PhoneNumber: user.PhoneNumber,
            Email: user.Email!,
            OnlyWhatsAppMessages: user.ReceivesOnlyWhatsAppMessages,
            DefaultAdoptionFormUrl: user.DefaultAdoptionFormUrl
        );
    }

    public static UserResponse ToUserResponse(this User user, TokensResponse tokens)
    {
        return new UserResponse(
            Id: user.Id,
            FullName: user.FullName,
            Email: user.Email!,
            Image: user.Image,
            PhoneNumber: user.PhoneNumber!,
            OnlyWhatsAppMessages: user.ReceivesOnlyWhatsAppMessages,
            AccessToken: tokens.AccessToken,
            RefreshToken: tokens.RefreshToken
        );
    }
}