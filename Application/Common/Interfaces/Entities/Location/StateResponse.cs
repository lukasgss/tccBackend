using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.Entities.Location;

[ExcludeFromCodeCoverage]
public sealed record LocationResponse(int Value, string Text);