namespace Alphadigi_migration.Domain.Common;


public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public abstract string EventType { get; }
    public abstract Guid AggregateId { get; }

    protected DomainEvent()
    {
      
    }
}