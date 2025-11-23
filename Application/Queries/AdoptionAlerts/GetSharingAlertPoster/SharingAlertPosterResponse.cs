using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.AdoptionAlerts.GetSharingAlertPoster;

[ExcludeFromCodeCoverage]
public record SharingAlertPosterResponse(MemoryStream PosterFile, string PetName);