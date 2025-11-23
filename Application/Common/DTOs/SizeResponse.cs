using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed class SizeResponse
{
    [SetsRequiredMembers]
    public SizeResponse(Size id, string name)
    {
        Id = id;
        Name = name;
    }

    public required Size Id { get; init; }
    public required string Name { get; init; }
}