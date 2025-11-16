using Application.Common.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMappings
{
    extension(User user)
    {
        public OwnerResponse ToOwnerResponse()
        {
            return new OwnerResponse(
                user.FullName,
                Image: user.Image,
                PhoneNumber: user.PhoneNumber!,
                Email: user.Email!
            );
        }

        public UserDataResponse ToUserDataResponse()
        {
            return new UserDataResponse(
                user.Id,
                FullName: user.FullName,
                Image: user.Image,
                PhoneNumber: user.PhoneNumber,
                Email: user.Email!,
                OnlyWhatsAppMessages: user.ReceivesOnlyWhatsAppMessages,
                DefaultAdoptionFormUrl: user.DefaultAdoptionFormUrl
            );
        }

        public UserResponse ToUserResponse(TokensResponse tokens)
        {
            return new UserResponse(
                user.Id,
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
}