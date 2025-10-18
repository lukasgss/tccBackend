namespace Application.Common.Interfaces.SharingAlerts;

public interface ISharingPosterGenerator
{
    Task<MemoryStream> GenerateAdoptionPoster(AdoptionAlertPosterData adoptionAlertPosterData);
}