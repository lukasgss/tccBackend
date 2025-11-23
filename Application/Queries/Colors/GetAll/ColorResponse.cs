using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.Colors.GetAll;

[ExcludeFromCodeCoverage]
public record ColorResponse(
    int Id,
    string Name,
    string HexCode
);