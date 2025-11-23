using System.Diagnostics.CodeAnalysis;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record OwnerResponse(
    string FullName,
    string Email,
    string Image,
    string PhoneNumber
);