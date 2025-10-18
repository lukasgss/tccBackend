namespace Infrastructure.ExternalServices.GeoLocation;

public record ViaCepResponse(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Localidade,
    string Uf
);