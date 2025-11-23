using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Alerts;

namespace Application.Queries.MissingAlerts.ListMissingAlerts;

[ExcludeFromCodeCoverage]
public class MissingAlertFilters : BaseAlertFilters
{
    public bool Missing { get; init; } = true;
    public bool NotMissing { get; init; }
}