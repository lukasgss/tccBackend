using System.Diagnostics.CodeAnalysis;
using Domain.Enums;

namespace Application.Common.DTOs;

public class AgeResponse
{
    [SetsRequiredMembers]
    public AgeResponse(Age id, string name)
    {
        Id = id;
        Name = name;
    }

    public required Age Id { get; init; }
    public required string Name { get; init; }
}