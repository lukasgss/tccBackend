namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class AdoptionAlertFilters : BaseAlertFilters
{
    public string? City { get; init; }
    public bool Adopted { get; set; }
    public bool NotAdopted { get; set; } = true;
}