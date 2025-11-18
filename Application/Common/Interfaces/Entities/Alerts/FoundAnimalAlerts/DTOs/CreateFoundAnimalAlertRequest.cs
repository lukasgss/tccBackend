using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public sealed class CreateFoundAnimalAlertRequest
{
	public string? Name { get; set; }
	public string? Description { get; set; }
	public int State { get; set; }
	public int City { get; set; }
	public string Neighborhood { get; set; } = null!;
	public Age Age { get; set; }
	public Size Size { get; set; }
	public int SpeciesId { get; set; }
	public int? BreedId { get; set; }
	public Gender? Gender { get; set; }
	public List<int> ColorIds { get; set; } = [];
	public List<IFormFile> Images { get; set; } = [];
}