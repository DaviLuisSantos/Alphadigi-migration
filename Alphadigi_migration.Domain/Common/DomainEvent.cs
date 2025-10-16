namespace Alphadigi_migration.Domain.Common;


public abstract class DomainEvent : IDomainEvent
{
    public int EventId { get; } 
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public abstract string EventType { get; }
    public abstract int AggregateId { get; }

  
}