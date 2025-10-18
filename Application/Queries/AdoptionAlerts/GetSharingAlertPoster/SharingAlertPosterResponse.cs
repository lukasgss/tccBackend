namespace Application.Queries.AdoptionAlerts.GetSharingAlertPoster;

public record SharingAlertPosterResponse(MemoryStream PosterFile, string PetName);