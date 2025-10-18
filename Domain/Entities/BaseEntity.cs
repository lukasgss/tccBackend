using Domain.Common;

namespace Domain.Entities;

public abstract class BaseEntity : IHasDomainEvents
{
    protected readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}