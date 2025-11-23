using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed class GenderResponse
{
    [SetsRequiredMembers]
    public GenderResponse(Gender id, string name)
    {
        Id = id;
        Name = name;
    }

    public required Gender Id { get; init; }
    public required string Name { get; init; }
}