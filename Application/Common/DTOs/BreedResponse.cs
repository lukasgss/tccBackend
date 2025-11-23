using System.Diagnostics.CodeAnalysis;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record BreedResponse(int Id, string Name);