using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.Users.Common;

[ExcludeFromCodeCoverage]
public sealed record UserDataResponse(
    Guid Id,
    string Image,
    string FullName,
    string Email,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages,
    string? DefaultAdoptionFormUrl
);