using Ardalis.GuardClauses;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _mediator;

    public PublishDomainEventsInterceptor(IPublisher mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await PublishDomainEvents(eventData.Context, inSameTransaction: true);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await PublishDomainEvents(eventData.Context, inSameTransaction: false);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishDomainEvents(DbContext? dbContext, bool inSameTransaction)
    {
        if (dbContext is null)
        {
            return;
        }

        var entitiesWithDomainEvents = dbContext.ChangeTracker.Entries<IHasDomainEvents>()
            .Where(entry => inSameTransaction
                ? entry.Entity.DomainEvents.Any(e => e.IsInSameTransaction)
                : entry.Entity.DomainEvents.Any(e => !e.IsInSameTransaction))
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entitiesWithDomainEvents
            .SelectMany(entry => entry.DomainEvents)
            .ToList();

        entitiesWithDomainEvents.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}