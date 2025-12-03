using CleanArcBase.Domain.Common;

namespace CleanArcBase.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<BaseEntity> entities, CancellationToken cancellationToken = default);
}
