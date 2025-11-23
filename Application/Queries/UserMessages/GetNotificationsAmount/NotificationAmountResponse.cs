using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.UserMessages.GetNotificationsAmount;

[ExcludeFromCodeCoverage]
public record NotificationAmountResponse(int Amount);