using Domain.Entities;

namespace Application.Common.Interfaces.General.Location;

public class AlertLocalization
{
	public required State State { get; init; }
	public required City City { get; init; }
}