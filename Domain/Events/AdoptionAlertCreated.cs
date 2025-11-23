using System.Diagnostics.CodeAnalysis;
using Domain.Common;
using Domain.Enums;

namespace Domain.Events;

[ExcludeFromCodeCoverage]
public sealed record AdoptionAlertCreated(
    Guid Id,
    Gender Gender,
    Age Age,
    Size Size,
    double? FoundLocationLatitude,
    double? FoundLocationLongitude,
    int SpeciesId,
    int BreedId,
    IEnumerable<int> ColorIds,
    bool IsInSameTransaction,
    Guid OwnerId) : IDomainEvent;