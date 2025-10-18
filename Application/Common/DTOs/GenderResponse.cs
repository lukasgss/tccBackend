using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Application.Common.DTOs;

public class GenderResponse
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