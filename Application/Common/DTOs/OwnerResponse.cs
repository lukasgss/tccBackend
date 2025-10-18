namespace Application.Common.DTOs;

public record OwnerResponse(
    string FullName,
    string Email,
    string Image,
    string PhoneNumber
);