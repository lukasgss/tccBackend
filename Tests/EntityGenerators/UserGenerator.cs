using Application.Commands.Users.Edit;
using Application.Common.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.EntityGenerators;

public static class UserGenerator
{
    public static User GenerateUser()
    {
        return new User()
        {
            Id = Constants.UserData.Id,
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            Image = Constants.UserData.ImageUrl,
            UserName = Constants.UserData.UserName,
            Email = Constants.UserData.Email,
            EmailConfirmed = Constants.UserData.EmailConfirmed
        };
    }

    public static User GenerateUserWithRandomId()
    {
        return new User()
        {
            Id = Guid.NewGuid(),
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            UserName = Constants.UserData.UserName,
            Image = Constants.UserData.ImageUrl,
            Email = Constants.UserData.Email,
            EmailConfirmed = Constants.UserData.EmailConfirmed
        };
    }

    public static EditUserRequest GenerateEditUserRequest()
    {
        return new EditUserRequest()
        {
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            Image = Constants.UserData.ImageFile
        };
    }

    public static UserDataResponse GenerateUserDataResponse()
    {
        return new UserDataResponse()
        {
            Id = Constants.UserData.Id,
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            Image = Constants.UserData.ImageUrl,
            Email = Constants.UserData.Email
        };
    }

    public static UserResponse GenerateUserResponseWithoutPhoneNumber()
    {
        return new UserResponse()
        {
            Id = Constants.UserData.Id,
            FullName = Constants.UserData.FullName,
            PhoneNumber = null,
            Image = Constants.UserData.ImageUrl,
            Email = Constants.UserData.Email,
            AccessToken = Constants.UserData.Tokens.AccessToken,
            RefreshToken = Constants.UserData.Tokens.RefreshToken
        };
    }
}