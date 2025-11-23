using System.Diagnostics.CodeAnalysis;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record SpeciesResponse(int Id, string Name);