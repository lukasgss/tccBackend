using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Interfaces.ExternalServices.GeoLocation;
using Application.Queries.GeoLocation.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalServices.GeoLocation;

public class GeoLocationClient : IGeoLocationClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GeoLocationClient> _logger;

    public GeoLocationClient(IHttpClientFactory httpClientFactory, ILogger<GeoLocationClient> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Application.Queries.GeoLocation.Common.GeoLocationResponse> GetCoordinatesViaPostalCodeAsync(
        string postalCode)
    {
        ViaCepResponse viaCepAddressData = await GetDataFromViaCep(postalCode);
        GeoLocationResponse addressData = await GetFullAddressDataFromViaCepData(viaCepAddressData);

        return new Application.Queries.GeoLocation.Common.GeoLocationResponse(
            Latitude: addressData.Lat,
            Longitude: addressData.Lon,
            Address: addressData.Address?.Road,
            PostalCode: addressData.Address?.PostalCode,
            State: addressData.Address?.State,
            City: addressData.Address?.City,
            Neighborhood: addressData.Address?.Neighborhood
        );
    }

    public async Task<Application.Queries.GeoLocation.Common.GeoLocationResponse?>
        GetCoordinatesFromNeighborhoodStateAndCityAsync(
            string neighborhood,
            string state,
            string city)
    {
        GeoLocationResponse? addressData = await GetFullAddressDataFromNeighborhood(neighborhood, state, city);

        if (addressData is null)
        {
            return null;
        }

        return new Application.Queries.GeoLocation.Common.GeoLocationResponse(
            Latitude: addressData.Lat,
            Longitude: addressData.Lon,
            Address: addressData.Address?.Road,
            PostalCode: addressData.Address?.PostalCode,
            State: addressData.Address?.State,
            City: addressData.Address?.City,
            Neighborhood: addressData.Address?.Neighborhood
        );
    }

    public async Task<Application.Queries.GeoLocation.Common.GeoLocationResponse?> GetLocationDataViaCoordinates(
        double latitude, double longitude)
    {
        GeoLocationResponse? locationData = await GetLocationDataFromCoordinates(latitude, longitude);
        if (locationData is null)
        {
            return null;
        }

        return new Application.Queries.GeoLocation.Common.GeoLocationResponse(
            Latitude: locationData.Lat,
            Longitude: locationData.Lon,
            Address: locationData.Address?.Road,
            PostalCode: locationData.Address?.PostalCode,
            State: locationData.Address?.State,
            City: locationData.Address?.City,
            Neighborhood: locationData.Address?.Neighborhood
        );
    }

    private async Task<ViaCepResponse> GetDataFromViaCep(string postalCode)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient(ViaCepApiConfig.ClientKey);
            ViaCepResponse? response = await client.GetFromJsonAsync<ViaCepResponse>($"/ws/{postalCode}/json");
            if (response is null)
            {
                _logger.LogInformation("Cep {Cep} retornou nulo na requisição via ViaCep", postalCode);
                throw new InternalServerErrorException("Não foi possível obter os dados do endereço.");
            }

            return response;
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BadRequestException("Formato do cep inválido, insira um cep válido e tente novamente.");
            }

            throw new InternalServerErrorException("Não foi possível obter os dados do endereço.");
        }
    }

    private async Task<GeoLocationResponse?> GetLocationDataFromCoordinates(double latitude, double longitude)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient(GeoLocationApiConfig.ClientKey);
            GeoLocationResponse? response = await client.GetFromJsonAsync<GeoLocationResponse>(
                $"/reverse?format=jsonv2&lat={latitude}&lon={longitude}&zoom=18&addressdetails=1");
            if (response is null)
            {
                _logger.LogInformation(
                    "Latitude e longitude {Latitude} - {Longitude} retornou nulo na requisição via coordenadas",
                    latitude,
                    longitude);
                throw new InternalServerErrorException("Não foi possível obter os dados da localização.");
            }

            return response;
        }
        catch (HttpRequestException)
        {
            throw new InternalServerErrorException("Não foi possível obter os dados do endereço.");
        }
        catch (JsonException)
        {
            _logger.LogInformation("Não foi possível obter a localização das coordenadas {Latitude}, {Longitude}",
                latitude, longitude);
            return null;
        }
    }

    private async Task<GeoLocationResponse?> GetFullAddressDataFromNeighborhood(
        string neighborhood, string state, string city)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient(GeoLocationApiConfig.ClientKey);

            string endpointUrl =
                $"/search?q={Uri.EscapeDataString(neighborhood)},{Uri.EscapeDataString(state)},{Uri.EscapeDataString(city)}&format=json&limit=1&featuretype=neighbourhood";

            var response = await client.GetFromJsonAsync<GeoLocationResponse[]>(endpointUrl);
            if (response is null || response.Length == 0)
            {
                _logger.LogInformation(
                    "Dados @{Neighborhood} retornou {NominationResponse} na requisição via Nomination. Endpoint utilizado foi {NominationEndpoint}",
                    neighborhood, response, endpointUrl);
                return null;
            }

            return response.First();
        }
        catch (HttpRequestException)
        {
            _logger.LogError("Não foi possível obter dados do bairro {Neighborhood}", neighborhood);
            return null;
        }
    }

    private async Task<GeoLocationResponse> GetFullAddressDataFromAddress(string address)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient(GeoLocationApiConfig.ClientKey);

            string[] splittedAddress = address.Split(",");
            string formattedAddress = splittedAddress.First();
            string? city = null;
            if (splittedAddress.Length >= 2)
            {
                city = splittedAddress[1].Trim();
            }

            string endpointUrl =
                $"/search?format=jsonv2&addressdetails=1&street={Uri.EscapeDataString(formattedAddress)}&limit=1&country=Brazil";
            if (!string.IsNullOrEmpty(city))
            {
                endpointUrl += $"&city={Uri.EscapeDataString(city)}";
            }

            var response = await client.GetFromJsonAsync<GeoLocationResponse[]>(endpointUrl);
            if (response is null || response.Length == 0)
            {
                _logger.LogInformation(
                    "Dados @{AddressData} retornou {NominationResponse} na requisição via Nomination. Endpoint utilizado foi {NominationEndpoint}",
                    address, response, endpointUrl);
                throw new NotFoundException("Não foi possível obter os dados do endereço.");
            }

            return response.First();
        }
        catch (HttpRequestException)
        {
            throw new InternalServerErrorException("Não foi possível obter os dados do endereço.");
        }
    }

    private async Task<GeoLocationResponse> GetFullAddressDataFromViaCepData(ViaCepResponse viaCepResponse)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient(GeoLocationApiConfig.ClientKey);
            var response = await client.GetFromJsonAsync<GeoLocationResponse[]>(
                $"/search?format=json&addressdetails=1&country=Brazil&state={viaCepResponse.Uf}&city={viaCepResponse.Localidade}&street={viaCepResponse.Logradouro}");
            if (response is null || response.Length == 0)
            {
                _logger.LogInformation(
                    "Dados @{AddressData} retornou {NominationResponse} na requisição via Nomination.",
                    viaCepResponse, response);
                throw new InternalServerErrorException("Não foi possível obter os dados do endereço.");
            }

            return response.First();
        }
        catch (HttpRequestException)
        {
            throw new InternalServerErrorException("Não foi possível obter os dados do endereço.");
        }
    }
}