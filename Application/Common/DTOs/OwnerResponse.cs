namespace Application.Common.DTOs;

public sealed record OwnerResponse(
    string FullName,
    string Email,
    string Image,
    string PhoneNumber
);