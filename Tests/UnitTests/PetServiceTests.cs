using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class PetServiceTests
{
    private readonly IPetRepository _petRepositoryMock;
    private readonly IBreedRepository _breedRepositoryMock;
    private readonly ISpeciesRepository _speciesRepositoryMock;
    private readonly IColorRepository _colorRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPetImageSubmissionService _imageSubmissionServiceMock;
    private readonly IValueProvider _valueProviderMock;
    private readonly IPetService _sut;

    private static readonly Pet Pet = PetGenerator.GeneratePet();
    private static readonly PetResponse ExpectedPetResponse = Pet.ToPetResponse();
    private static readonly User User = UserGenerator.GenerateUser();
    private static readonly Breed Breed = BreedGenerator.GenerateBreed();
    private static readonly Species Species = SpeciesGenerator.GenerateSpecies();
    private static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
    private static readonly CreatePetRequest CreatePetRequest = PetGenerator.GenerateCreatePetRequest();
    private static readonly EditPetRequest EditPetRequest = PetGenerator.GenerateEditPetRequest();

    public PetServiceTests()
    {
        _petRepositoryMock = Substitute.For<IPetRepository>();
        _breedRepositoryMock = Substitute.For<IBreedRepository>();
        _speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
        _colorRepositoryMock = Substitute.For<IColorRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _imageSubmissionServiceMock = Substitute.For<IPetImageSubmissionService>();
        _valueProviderMock = Substitute.For<IValueProvider>();
        var loggerMock = Substitute.For<ILogger<PetService>>();

        _sut = new PetService(
            _petRepositoryMock,
            _breedRepositoryMock,
            _speciesRepositoryMock,
            _colorRepositoryMock,
            _userRepositoryMock,
            _imageSubmissionServiceMock,
            _valueProviderMock,
            loggerMock);
    }

    [Fact]
    public async Task Get_Non_Existent_Pet_By_Id_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).ReturnsNull();

        async Task Result() => await _sut.GetPetBydIdAsync(Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Pet_By_Id_Returns_PetResponse()
    {
        _petRepositoryMock.GetPetByIdAsync(Arg.Any<Guid>()).Returns(Pet);

        PetResponse petResponse = await _sut.GetPetBydIdAsync(Pet.Id);

        Assert.Equivalent(ExpectedPetResponse, petResponse);
    }

    [Fact]
    public async Task Create_Pet_With_Non_Existent_Breed_Throws_NotFoundException()
    {
        _breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).ReturnsNull();

        async Task Result() => await _sut.GeneratePetToBeCreatedAsync(CreatePetRequest, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Raça especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Pet_With_Non_Existent_Species_Throws_NotFoundException()
    {
        _breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(CreatePetRequest.SpeciesId).ReturnsNull();

        async Task Result() => await _sut.GeneratePetToBeCreatedAsync(CreatePetRequest, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Espécie especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Pet_With_Non_Existent_Colors_Throws_NotFoundException()
    {
        _breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(CreatePetRequest.SpeciesId).Returns(Species);
        List<Color> emptyColorList = new();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(CreatePetRequest.ColorIds).Returns(emptyColorList);

        async Task Result() => await _sut.GeneratePetToBeCreatedAsync(CreatePetRequest, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Pet_While_Authenticated_Returns_Pet_Response_With_Logged_In_User_As_Owner()
    {
        _breedRepositoryMock.GetBreedByIdAsync(CreatePetRequest.BreedId).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(CreatePetRequest.SpeciesId).Returns(Species);
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(CreatePetRequest.ColorIds).Returns(Colors);
        _userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
        _valueProviderMock.NewGuid().Returns(Pet.Id);
        _imageSubmissionServiceMock.UploadPetImageAsync(CreatePetRequest.Images)
            .Returns(Constants.PetData.ImageUrls);
        _petRepositoryMock.CreatePetAndImages(Arg.Any<Pet>(), Arg.Any<List<PetImage>>()).Returns(true);

        Pet petResponse = await _sut.GeneratePetToBeCreatedAsync(CreatePetRequest, userId: User.Id);

        Assert.NotNull(petResponse);
    }

    [Fact]
    public async Task Edit_Pet_With_Route_Id_Different_Than_Specified_On_Request_Throws_BadRequestException()
    {
        async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, routeId: Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Edit_Non_Existent_Pet_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).ReturnsNull();

        async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_With_Non_Existent_Breed_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
        _breedRepositoryMock.GetBreedByIdAsync(EditPetRequest.BreedId).ReturnsNull();

        async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Raça especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_With_Non_Existent_Species_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
        _breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(EditPetRequest.SpeciesId).ReturnsNull();

        async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Espécie especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_With_Non_Existent_Colors_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
        _breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);
        List<Color> emptyColorsList = new();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(EditPetRequest.ColorIds).Returns(emptyColorsList);

        async Task Result() => await _sut.EditPetAsync(EditPetRequest, Constants.UserData.Id, EditPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_Without_Being_Owner_Throws_UnauthorizedException()
    {
        _petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
        _breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(EditPetRequest.ColorIds).Returns(Colors);
        _userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).Returns(User);
        Guid nonOwnerUserId = Guid.NewGuid();
        _userRepositoryMock.GetUserByIdAsync(nonOwnerUserId).Returns(new User());

        async Task Result() =>
            await _sut.EditPetAsync(EditPetRequest, userId: nonOwnerUserId, routeId: EditPetRequest.Id);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Você não possui permissão para editar dados desse animal.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_Returns_Edited_Pet_Response()
    {
        _petRepositoryMock.GetPetByIdAsync(EditPetRequest.Id).Returns(Pet);
        _breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(EditPetRequest.ColorIds).Returns(Colors);
        _userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
        _imageSubmissionServiceMock
            .UpdatePetImageAsync(EditPetRequest.Id, EditPetRequest.Images)
            .Returns(Constants.PetData.ImageUrls);

        PetResponse editedPetResponse = await _sut.EditPetAsync(EditPetRequest, User.Id, EditPetRequest.Id);

        Assert.Equivalent(ExpectedPetResponse, editedPetResponse);
    }

    [Fact]
    public async Task Delete_Non_Existent_Pet_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).ReturnsNull();

        async Task Result() => await _sut.DeletePetAsync(Constants.PetData.Id, Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Delete_Pet_Without_Being_Its_Owner_Throws_UnauthorizedException()
    {
        _petRepositoryMock.GetPetByIdAsync(Constants.PetData.Id).Returns(Pet);

        async Task Result() => await _sut.DeletePetAsync(Pet.Id, userId: Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Você não possui permissão para excluir o animal.", exception.Message);
    }
}