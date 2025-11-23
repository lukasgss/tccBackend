using System.Diagnostics.CodeAnalysis;
using Domain.Entities;

namespace Application.Common.Interfaces.General.Location;

[ExcludeFromCodeCoverage]
public sealed class AlertLocalization
{
	public required State State { get; init; }
	public required City City { get; init; }
}