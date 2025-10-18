using System.Text.Json.Serialization;

namespace Infrastructure.ExternalServices.GeoLocation;

public class GeoLocationResponse
{
    [JsonPropertyName("lat")] public required string Lat { get; init; }

    [JsonPropertyName("lon")] public required string Lon { get; init; }

    [JsonPropertyName("addresstype")] public string? Addresstype { get; init; }

    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("display_name")] public string? DisplayName { get; init; }

    [JsonPropertyName("address")] public Address? Address { get; init; }
}

public class Address
{
    [JsonPropertyName("road")] public string? Road { get; init; }

    [JsonPropertyName("neighbourhood")] public string? Neighborhood { get; init; }

    [JsonPropertyName("city")] public string? City { get; init; }

    [JsonPropertyName("state")] public string? State { get; init; }

    [JsonPropertyName("postcode")] public string? PostalCode { get; init; }
}