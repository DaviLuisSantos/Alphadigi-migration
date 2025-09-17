

namespace Alphadigi_migration.Domain.Common;

public abstract class DomainEventBase : IDomainEvent
{
    private static int _counter = 0; 

    public int EventId { get; } = Interlocked.Increment(ref _counter);
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
