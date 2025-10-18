using Application.Commands.Users.Edit;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.Providers;
using Application.Queries.Users.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.Fakes.Identity;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class UserServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly ITokenGenerator _tokenGeneratorMock;
    private readonly IIdConverterService _idConverterServiceMock;
    private readonly IUserImageSubmissionService _userImageSubmissionServiceMock;
    private readonly IExternalAuthHandler _externalAuthHandlerMock;
    private readonly IValueProvider _valueProviderMock;
    private readonly IUserService _sut;

    private static readonly User User = UserGenerator.GenerateUser();
    private static readonly UserDataResponse UserDataResponse = User.ToUserDataResponse();
    private static readonly UserResponse ExpectedUserResponse = User.ToUserResponse(Constants.UserData.Tokens);

    private static readonly UserResponse ExpectedUserResponseWithoutPhoneNumber =
        UserGenerator.GenerateUserResponseWithoutPhoneNumber();

    private static readonly CreateUserRequest CreateUserRequest = UserGenerator.GenerateCreateUserRequest();
    private static readonly LoginUserRequest LoginUserRequest = UserGenerator.GenerateLoginUserRequest();
    private static readonly EditUserRequest EditUserRequest = UserGenerator.GenerateEditUserRequest();

    private static readonly ExternalAuthPayload ExternalAuthPayload = new()
    {
        UserId = User.Id.ToString(),
        Email = User.Email!,
        Image = User.Image,
        FullName = User.FullName
    };

    private static readonly ExternalAuthRequest
        ExternalAuthRequest = new() { Provider = "Google", IdToken = "IdToken" };

    public UserServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _tokenGeneratorMock = Substitute.For<ITokenGenerator>();
        IHttpContextAccessor httpRequestMock = Substitute.For<IHttpContextAccessor>();
        IMessagingService messagingServiceMock = Substitute.For<IMessagingService>();
        LinkGenerator linkGeneratorMock = Substitute.For<LinkGenerator>();
        _idConverterServiceMock = Substitute.For<IIdConverterService>();
        _userImageSubmissionServiceMock = Substitute.For<IUserImageSubmissionService>();
        _externalAuthHandlerMock = Substitute.For<IExternalAuthHandler>();
        _valueProviderMock = Substitute.For<IValueProvider>();
        var loggerMock = Substitute.For<ILogger<UserService>>();

        _sut = new UserService(
            _userRepositoryMock,
            _tokenGeneratorMock,
            httpRequestMock,
            messagingServiceMock,
            linkGeneratorMock,
            _idConverterServiceMock,
            _userImageSubmissionServiceMock,
            _externalAuthHandlerMock,
            _valueProviderMock,
            loggerMock);
    }

    [Fact]
    public async Task Get_Non_Existent_User_By_Id_Throws_NotFoundException()
    {
        _userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).ReturnsNull();

        async Task Result() => await _sut.GetUserByIdAsync(Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Não foi possível obter o usuário com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Get_User_By_Id_Returns_User_Data()
    {
        _userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);

        UserDataResponse userResponse = await _sut.GetUserByIdAsync(User.Id);

        Assert.Equivalent(UserDataResponse, userResponse);
    }

    [Fact]
    public async Task Register_Attempt_With_Already_Existing_Email_Throws_ConflictException()
    {
        _valueProviderMock.NewGuid().Returns(User.Id);
        _userRepositoryMock.GetUserByEmailAsync(CreateUserRequest.Email).Returns(User);
        _userImageSubmissionServiceMock.UploadUserImageAsync(User.Id, null, null).Returns(User.Image);

        async Task Result() => await _sut.RegisterAsync(CreateUserRequest);

        var exception = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Usuário com o e-mail especificado já existe.", exception.Message);
    }

    [Fact]
    public async Task Register_Attempt_With_Any_Registration_Error_Throws_InternalServerErrorException()
    {
        _valueProviderMock.NewGuid().Returns(Constants.UserData.Id);
        _userImageSubmissionServiceMock.UploadUserImageAsync(User.Id, null, null).Returns(User.Image);
        _userRepositoryMock.GetUserByEmailAsync(Constants.UserData.Email).ReturnsNull();
        IdentityResult expectedIdentityResult = new FakeIdentityResult(succeeded: false);
        _userRepositoryMock.RegisterUserAsync(Arg.Any<User>(), CreateUserRequest.Password)
            .Returns(expectedIdentityResult);
        _userRepositoryMock.SetLockoutEnabledAsync(Arg.Any<User>(), false).Returns(expectedIdentityResult);

        async Task Result() => await _sut.RegisterAsync(CreateUserRequest);

        await Assert.ThrowsAsync<InternalServerErrorException>(Result);
    }

    [Fact]
    public async Task Google_External_Login_With_Invalid_Request_Throws_BadRequestException()
    {
        _externalAuthHandlerMock.ValidateGoogleToken(ExternalAuthRequest).ReturnsNull();

        async Task Result() => await _sut.ExternalLoginAsync(ExternalAuthRequest);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível realizar o login com o Google.", exception.Message);
    }

    [Fact]
    public async Task Google_Login_With_Existing_Oauth_User_Returns_User_Data()
    {
        _externalAuthHandlerMock.ValidateGoogleToken(ExternalAuthRequest).Returns(ExternalAuthPayload);
        UserLoginInfo info = new(ExternalAuthRequest.Provider, ExternalAuthPayload.UserId,
            ExternalAuthRequest.Provider);
        _userRepositoryMock.FindByLoginAsync(info.LoginProvider, info.ProviderKey).Returns(User);
        _userRepositoryMock.FindByEmailAsync(ExternalAuthPayload.Email).Returns(User);
        _tokenGeneratorMock.GenerateTokens(User.Id, User.FullName).Returns(Constants.UserData.Tokens);

        UserResponse userResponse = await _sut.ExternalLoginAsync(ExternalAuthRequest);

        Assert.Equivalent(ExpectedUserResponse, userResponse);
    }

    [Fact]
    public async Task Google_Login_With_Non_Existent_Oauth_User_Returns_User_Data()
    {
        _externalAuthHandlerMock.ValidateGoogleToken(ExternalAuthRequest).Returns(ExternalAuthPayload);
        UserLoginInfo info = new(ExternalAuthRequest.Provider, ExternalAuthPayload.UserId,
            ExternalAuthRequest.Provider);
        _userRepositoryMock.FindByLoginAsync(info.LoginProvider, info.ProviderKey).ReturnsNull();
        _userRepositoryMock.FindByEmailAsync(ExternalAuthPayload.Email).ReturnsNull();
        _valueProviderMock.NewGuid().Returns(User.Id);
        _tokenGeneratorMock.GenerateTokens(User.Id, User.FullName)
            .Returns(Constants.UserData.Tokens);
        _userImageSubmissionServiceMock.ValidateUserImage(ExternalAuthPayload.Image).Returns(User.Image);

        UserResponse userResponse = await _sut.ExternalLoginAsync(ExternalAuthRequest);

        Assert.Equivalent(ExpectedUserResponseWithoutPhoneNumber, userResponse);
    }

    [Fact]
    public async Task Google_Login_With_Existing_User_Registered_Without_Oauth_Returns_User_Data()
    {
        _externalAuthHandlerMock.ValidateGoogleToken(ExternalAuthRequest).Returns(ExternalAuthPayload);
        UserLoginInfo info = new(ExternalAuthRequest.Provider, ExternalAuthPayload.UserId,
            ExternalAuthRequest.Provider);
        _userRepositoryMock.FindByLoginAsync(info.LoginProvider, info.ProviderKey).ReturnsNull();
        _userRepositoryMock.FindByEmailAsync(ExternalAuthPayload.Email).Returns(User);
        _tokenGeneratorMock.GenerateTokens(User.Id, User.FullName).Returns(Constants.UserData.Tokens);

        UserResponse userResponse = await _sut.ExternalLoginAsync(ExternalAuthRequest);

        Assert.Equivalent(ExpectedUserResponse, userResponse);
    }


    [Fact]
    public async Task Edit_User_With_Id_Different_Than_Route_Throws_BadRequestException()
    {
        Guid differentRouteId = Guid.NewGuid();

        async Task Result() => await _sut.EditAsync(EditUserRequest, User.Id, differentRouteId);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Edit_Another_User_Throws_ForbiddenException()
    {
        Guid differentUserId = Guid.NewGuid();

        async Task Result() => await _sut.EditAsync(EditUserRequest, differentUserId, EditUserRequest.Id);

        var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
        Assert.Equal("Você não possui permissão para editar este usuário.", exception.Message);
    }

    [Fact]
    public async Task Edit_Non_Existent_User_Throws_NotFoundException()
    {
        _userRepositoryMock.GetUserByIdAsync(EditUserRequest.Id).ReturnsNull();

        async Task Result() => await _sut.EditAsync(EditUserRequest, User.Id, EditUserRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Usuário com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_User_Returns_Edited_user()
    {
        _userRepositoryMock.GetUserByIdAsync(EditUserRequest.Id).Returns(User);
        UserDataResponse expectedUserResponse = UserGenerator.GenerateUserDataResponse();
        _userImageSubmissionServiceMock.UploadUserImageAsync(User.Id, EditUserRequest.Image, Arg.Any<string>())
            .Returns(expectedUserResponse.Image);

        UserDataResponse editedUser = await _sut.EditAsync(EditUserRequest, User.Id, EditUserRequest.Id);

        Assert.Equivalent(expectedUserResponse, editedUser);
    }

    [Fact]
    public async Task Registration_Returns_User_Response()
    {
        // Arrange
        _valueProviderMock.NewGuid().Returns(Constants.UserData.Id);
        _userImageSubmissionServiceMock.UploadUserImageAsync(User.Id, null, null).Returns(User.Image);
        _userRepositoryMock.GetUserByEmailAsync(Constants.UserData.Email).ReturnsNull();

        IdentityResult expectedIdentityResult = new FakeIdentityResult(succeeded: true);
        _userRepositoryMock.RegisterUserAsync(Arg.Any<User>(), CreateUserRequest.Password)
            .Returns(expectedIdentityResult);
        _userRepositoryMock.SetLockoutEnabledAsync(Arg.Any<User>(), false)
            .Returns(expectedIdentityResult);
        _tokenGeneratorMock.GenerateTokens(User.Id, Constants.UserData.FullName)
            .Returns(Constants.UserData.Tokens);

        // Act
        UserResponse userResponse = await _sut.RegisterAsync(CreateUserRequest);

        // Assert
        Assert.Equivalent(ExpectedUserResponse, userResponse);
    }

    [Fact]
    public async Task Login_With_Locked_Account_Throws_LockedException()
    {
        _userRepositoryMock.GetUserByEmailAsync(LoginUserRequest.Email).Returns(User);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: true);
        _userRepositoryMock.CheckCredentials(User, LoginUserRequest.Password)
            .Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(LoginUserRequest);

        var exception = await Assert.ThrowsAsync<LockedException>(Result);
        Assert.Equal("Essa conta está bloqueada, aguarde e tente novamente.", exception.Message);
    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Throws_UnauthorizedException()
    {
        _userRepositoryMock.GetUserByEmailAsync(LoginUserRequest.Email).Returns(User);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: false);
        _userRepositoryMock.CheckCredentials(User, LoginUserRequest.Password)
            .Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(LoginUserRequest);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_UserResponse()
    {
        _userRepositoryMock.GetUserByEmailAsync(LoginUserRequest.Email).Returns(User);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: true, isLockedOut: false);
        _userRepositoryMock.CheckCredentials(Arg.Any<User>(), LoginUserRequest.Password)
            .Returns(fakeSignInResult);
        _tokenGeneratorMock.GenerateTokens(User.Id, User.FullName).Returns(Constants.UserData.Tokens);

        UserResponse userResponse = await _sut.LoginAsync(LoginUserRequest);

        Assert.Equivalent(ExpectedUserResponse, userResponse);
    }

    [Fact]
    public async Task Confirm_Email_With_Invalid_User_Id_Throws_BadRequestException()
    {
        Guid decodedUserId = Guid.NewGuid();
        _idConverterServiceMock.DecodeShortIdToGuid(Arg.Any<string>()).Returns(decodedUserId);
        _userRepositoryMock.GetUserByIdAsync(decodedUserId).ReturnsNull();

        async Task Result() => await _sut.ConfirmEmailAsync("hashedUserId", "token");

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível ativar o email com os dados informados.", exception.Message);
    }

    [Fact]
    public async Task Confirm_Email_With_Confirmation_Fail_Throws_BadRequestException()
    {
        Guid decodedUserId = Guid.NewGuid();
        _idConverterServiceMock.DecodeShortIdToGuid(Arg.Any<string>()).Returns(decodedUserId);
        _userRepositoryMock.GetUserByIdAsync(decodedUserId).Returns(User);
        FakeIdentityResult fakeIdentityResult = new(succeeded: false);
        _userRepositoryMock.ConfirmEmailAsync(User, "token").Returns(fakeIdentityResult);

        async Task Result() => await _sut.ConfirmEmailAsync("hashedUserId", "token");

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível ativar o email com os dados informados.", exception.Message);
    }
}