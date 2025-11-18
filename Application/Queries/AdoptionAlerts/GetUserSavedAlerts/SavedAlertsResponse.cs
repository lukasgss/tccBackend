using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

namespace Application.Queries.AdoptionAlerts.GetUserSavedAlerts;

public sealed record SavedAlertsResponse(
	List<SavedAdoptionListingResponse> AdoptionAlerts,
	List<FoundAnimalAlertResponse> FoundAnimalAlerts);