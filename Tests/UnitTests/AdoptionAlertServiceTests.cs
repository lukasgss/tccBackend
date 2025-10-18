using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.Providers;
using Application.Queries.AdoptionFavorites.GetById;
using Application.Queries.GeoLocation.Common;
using Application.Services.General.Messages;
using Domain.Entities;
using Domain.Entities.Alerts;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class AdoptionAlertServiceTests
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepositoryMock;
    private readonly IPetRepository _petRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IGeoLocationService _geoLocationServiceMock;
    private readonly IValueProvider _valueProviderMock;
    private readonly IPetService _petServiceMock;
    private readonly ILocalizationRepository _localizationRepositoryMock;
    private readonly IAdoptionAlertService _sut;

    private const int Page = 1;
    private const int PageSize = 25;

    private static readonly AdoptionAlert NonAdoptedAdoptionAlert =
        AdoptionAlertGenerator.GenerateNonAdoptedAdoptionAlert();

    private static readonly AdoptionAlert AdoptedAdoptionAlert = AdoptionAlertGenerator.GenerateAdoptedAdoptionAlert();
    private static readonly Pet Pet = PetGenerator.GeneratePet();
    private static readonly User User = UserGenerator.GenerateUser();

    private static readonly CreateAdoptionAlertCommand CreateAlertCommand =
        AdoptionAlertGenerator.GenerateCreateAdoptionAlertRequest();

    private static readonly EditAdoptionAlertRequest EditAlertRequest =
        AdoptionAlertGenerator.GenerateEditAdoptionAlertRequest();

    private static readonly AdoptionAlertResponse
        AdoptedAdoptionAlertResponse = AdoptionAlertGenerator.GenerateAdoptedAdoptionAlertResponse();

    private static readonly AdoptionAlertResponse
        NonAdoptedAdoptionAlertResponse = AdoptionAlertGenerator.GenerateNonAdoptedAdoptionAlertResponse();

    private static readonly AdoptionAlertResponseWithGeoLocation
        AdoptionAlertResponseWithGeoLocation = AdoptionAlertGenerator.GenerateAdoptionAlertResponseWithGeoLocation();

    private static readonly AdoptionAlertFilters AdoptionAlertFilters =
        AdoptionAlertGenerator.GenerateAdotionAlertFilters();

    private static readonly State State = new();
    private static readonly City City = new();

    private static readonly GeoLocationResponse GeoLocationResponse =
        AdoptionAlertGenerator.GenerateGeoLocationResponse();

    public AdoptionAlertServiceTests()
    {
        _adoptionAlertRepositoryMock = Substitute.For<IAdoptionAlertRepository>();
        _petRepositoryMock = Substitute.For<IPetRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        var alertsMessagingServiceMock = Substitute.For<IAlertsMessagingService>();
        _geoLocationServiceMock = Substitute.For<IGeoLocationService>();
        _valueProviderMock = Substitute.For<IValueProvider>();
        _petServiceMock = Substitute.For<IPetService>();
        _localizationRepositoryMock = Substitute.For<ILocalizationRepository>();
        var breedRepositoryMock = Substitute.For<IBreedRepository>();
        var speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
        var colorRepositoryMock = Substitute.For<IColorRepository>();
        var petImageRepositoryMock = Substitute.For<IPetImageRepository>();
        var petImageSubmissionServiceMock = Substitute.For<IPetImageSubmissionService>();
        var loggerMock = Substitute.For<ILogger<AdoptionAlertService>>();

        _sut = new AdoptionAlertService(
            _adoptionAlertRepositoryMock,
            _petRepositoryMock,
            _userRepositoryMock,
            alertsMessagingServiceMock,
            _geoLocationServiceMock,
            _valueProviderMock,
            _petServiceMock,
            _localizationRepositoryMock,
            breedRepositoryMock,
            speciesRepositoryMock,
            colorRepositoryMock,
            petImageRepositoryMock,
            petImageSubmissionServiceMock,
            loggerMock);
    }

    [Fact]
    public async Task Get_Non_Existent_Alert_By_Id_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).ReturnsNull();

        async Task Result() => await _sut.GetByIdAsync(NonAdoptedAdoptionAlert.Id, User.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de adoção com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Adoption_Alert_By_Id_Returns_Alert()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).Returns(NonAdoptedAdoptionAlert);
        GeoLocationResponse geoLocationResponse = AlertGeoLocationGenerator.GenerateGeoLocationResponse();
        _geoLocationServiceMock
            .GetLocationDataFromCoordinatesAsync(AdoptedAdoptionAlert.Location?.Y, AdoptedAdoptionAlert.Location?.X)
            .Returns(geoLocationResponse);

        AdoptionAlertResponseWithGeoLocation
            adoptionAlertResponse = await _sut.GetByIdAsync(NonAdoptedAdoptionAlert.Id, User.Id);

        Assert.Equivalent(AdoptionAlertResponseWithGeoLocation, adoptionAlertResponse);
    }

    [Fact]
    public async Task Get_Adoption_Alerts_With_Page_Less_Than_1_Throws_BadRequestException()
    {
        async Task Result() => await _sut.ListAdoptionAlerts(AdoptionAlertFilters, User.Id, 0, 25);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Insira um número e tamanho de página maior ou igual a 1.", exception.Message);
    }

    [Fact]
    public async Task Get_Adoption_Alerts_With_Page_Size_Less_Than_1_Throws_BadRequestException()
    {
        async Task Result() => await _sut.ListAdoptionAlerts(new AdoptionAlertFilters(), User.Id, page: 1, pageSize: 0);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Insira um número e tamanho de página maior ou igual a 1.", exception.Message);
    }

    [Fact]
    public async Task List_Adoption_Alerts_Returns_Adoption_Alerts()
    {
        var pagedAdoptionAlerts = PagedListGenerator.GeneratePagedAdoptionAlerts();
        _adoptionAlertRepositoryMock.ListAdoptionAlerts(AdoptionAlertFilters, Page, PageSize, User.Id)
            .Returns(pagedAdoptionAlerts);
        var expectedAlerts = PaginatedEntityGenerator.GeneratePaginatedAdoptionAlertResponse();

        var adoptionAlertsResponse = await _sut.ListAdoptionAlerts(AdoptionAlertFilters, User.Id, Page, PageSize);

        Assert.Equivalent(expectedAlerts, adoptionAlertsResponse);
    }

    [Fact]
    public async Task Create_Adoption_Alert_With_Non_Existent_State_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);
        _userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).Returns(User);
        _localizationRepositoryMock.GetStateById(CreateAlertCommand.State).ReturnsNull();

        async Task Result() => await _sut.CreateAsync(CreateAlertCommand, Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Estado especificado não foi encontrado.", exception.Message);
    }

    [Fact]
    public async Task Create_Adoption_Alert_With_Non_Existent_City_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);
        _userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).Returns(User);
        _localizationRepositoryMock.GetStateById(CreateAlertCommand.State).Returns(State);
        _localizationRepositoryMock.GetCityById(CreateAlertCommand.City).ReturnsNull();

        async Task Result() => await _sut.CreateAsync(CreateAlertCommand, Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Cidade especificada não foi encontrada.", exception.Message);
    }

    [Fact]
    public async Task Create_Adoption_Alert_Returns_Created_Alert()
    {
        _userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);
        _localizationRepositoryMock.GetStateById(CreateAlertCommand.State).Returns(State);
        _localizationRepositoryMock.GetCityById(CreateAlertCommand.City).Returns(City);
        _geoLocationServiceMock
            .GetCoordinatesFromAddressStateAndCityAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(GeoLocationResponse);
        _petServiceMock.GeneratePetToBeCreatedAsync(CreateAlertCommand.Pet, User.Id).Returns(Pet);

        _valueProviderMock.NewGuid().Returns(Constants.AdoptionAlertData.Id);
        _valueProviderMock.UtcNow().Returns(Constants.AdoptionAlertData.RegistrationDate);
        _petServiceMock.UploadPetImages(Arg.Any<Pet>(), CreateAlertCommand.Pet.Images).Returns([]);
        _adoptionAlertRepositoryMock.CreateAlertAndPet(Arg.Any<AdoptionAlert>(), Pet).Returns(true);

        AdoptionAlertResponse adoptionAlertResponse = await _sut.CreateAsync(CreateAlertCommand, Constants.UserData.Id);

        Assert.Equivalent(NonAdoptedAdoptionAlertResponse, adoptionAlertResponse);
    }

    [Fact]
    public async Task Edit_Adoption_With_Id_Different_Than_Specified_In_Route_Throws_BadRequestException()
    {
        Guid differentId = Guid.NewGuid();
        async Task Result() => await _sut.EditAsync(EditAlertRequest, User.Id, differentId);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Edit_Non_Existent_Adoption_Alert_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(EditAlertRequest.Id).ReturnsNull();

        async Task Result() => await _sut.EditAsync(EditAlertRequest, User.Id, EditAlertRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de adoção com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Adoption_Alert_Without_Being_Owner_Of_Alert_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(EditAlertRequest.Id).Returns(NonAdoptedAdoptionAlert);
        Guid idOfUserThatDoesntOwnAlert = Guid.NewGuid();

        async Task Result() => await _sut.EditAsync(EditAlertRequest, idOfUserThatDoesntOwnAlert, EditAlertRequest.Id);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível alterar alertas de adoção de outros usuários.", exception.Message);
    }

    [Fact]
    public async Task Edit_Adoption_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(EditAlertRequest.Id).Returns(NonAdoptedAdoptionAlert);
        _petRepositoryMock.GetPetByIdAsync(EditAlertRequest.Pet.Id).ReturnsNull();

        async Task Result() => await _sut.EditAsync(EditAlertRequest, User.Id, EditAlertRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Adoption_Alert_Without_Being_Owner_Of_Pet_Throws_UnauthorizedException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(EditAlertRequest.Id).Returns(NonAdoptedAdoptionAlert);
        Pet petThatUserDoesNotOwn = PetGenerator.GeneratePetWithRandomOwnerId();
        _petRepositoryMock.GetPetByIdAsync(EditAlertRequest.Pet.Id).Returns(petThatUserDoesNotOwn);

        async Task Result() => await _sut.EditAsync(EditAlertRequest, User.Id, EditAlertRequest.Id);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível cadastrar ou editar adoções para animais em que não é dono.", exception.Message);
    }

    [Fact]
    public async Task Delete_Non_Existent_Adoption_Alert_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).ReturnsNull();

        async Task Result() => await _sut.DeleteAsync(NonAdoptedAdoptionAlert.Id, User.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de adoção com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Delete_Adoption_Alert_Without_Being_Owner_Of_Alert_Throws_UnauthorizedException()
    {
        AdoptionAlert adoptionAlertWithRandomOwner = AdoptionAlertGenerator.GenerateAdoptionAlertWithRandomOwner();
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).Returns(adoptionAlertWithRandomOwner);

        async Task Result() => await _sut.DeleteAsync(adoptionAlertWithRandomOwner.Id, User.Id);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Não é possível alterar alertas de adoção de outros usuários.", exception.Message);
    }

    [Fact]
    public async Task Toggle_Non_Existent_Adoption_Alert_Throws_NotFoundException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).ReturnsNull();

        async Task Result() => await _sut.ToggleAdoptionAsync(NonAdoptedAdoptionAlert.Id, User.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alerta de adoção com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Toggle_Adoption_Alert_Without_Being_Its_Owner_Throws_ForbiddenException()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).Returns(NonAdoptedAdoptionAlert);
        Guid idOfUserThatDoesNotOwnAlert = Guid.NewGuid();

        async Task Result() => await _sut.ToggleAdoptionAsync(NonAdoptedAdoptionAlert.Id, idOfUserThatDoesNotOwnAlert);

        var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
        Assert.Equal("Não é possível alterar o status de alertas em que não é dono.", exception.Message);
    }

    [Fact]
    public async Task Toggle_Adoption_Alert_Not_Adopted_Before_Returns_Adopted_Response()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(NonAdoptedAdoptionAlert.Id).Returns(NonAdoptedAdoptionAlert);
        _valueProviderMock.DateOnlyNow().Returns(Constants.AdoptionAlertData.AdoptedAdoptionDate);

        AdoptionAlertResponse adoptionAlertResponse =
            await _sut.ToggleAdoptionAsync(NonAdoptedAdoptionAlert.Id, User.Id);

        Assert.Equivalent(AdoptedAdoptionAlertResponse, adoptionAlertResponse);
    }

    [Fact]
    public async Task Toggle_Previously_Adopted_Alert_Returns_Non_Adopted_Alert()
    {
        _adoptionAlertRepositoryMock.GetByIdAsync(AdoptedAdoptionAlert.Id).Returns(AdoptedAdoptionAlert);

        AdoptionAlertResponse adoptionAlertResponse = await _sut.ToggleAdoptionAsync(AdoptedAdoptionAlert.Id, User.Id);

        Assert.Equivalent(NonAdoptedAdoptionAlertResponse, adoptionAlertResponse);
    }
}