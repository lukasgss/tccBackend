namespace Application.Common.Interfaces.FrontendDropdownData;

public class DropdownDataResponse<T> where T : notnull
{
    public required string Label { get; init; } = null!;
    public required T Value { get; init; }
}