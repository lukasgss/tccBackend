using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.FrontendDropdownData;

[ExcludeFromCodeCoverage]
public sealed class DropdownDataResponse<T> where T : notnull
{
    public required string Label { get; init; } = null!;
    public required T Value { get; init; }
}