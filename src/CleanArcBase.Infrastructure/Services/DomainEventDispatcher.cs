using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Domain.Common;
using MediatR;

namespace CleanArcBase.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchEventsAsync(IEnumerable<BaseEntity> entities, CancellationToken cancellationToken = default)
    {
        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
