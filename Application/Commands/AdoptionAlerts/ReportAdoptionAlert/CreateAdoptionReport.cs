using System.Diagnostics.CodeAnalysis;

namespace Application.Commands.AdoptionAlerts.ReportAdoptionAlert;

[ExcludeFromCodeCoverage]
public sealed record CreateAdoptionReport(string Reason);