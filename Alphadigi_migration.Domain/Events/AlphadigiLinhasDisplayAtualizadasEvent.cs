
using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events;


public class AlphadigiLinhasDisplayAtualizadasEvent : DomainEvent
{
    public int AlphadigiId { get; }
    public string AlphadigiNome { get; }
    public int NovasLinhasDisplay { get; }

    public override string EventType => throw new NotImplementedException();

    public override Guid AggregateId => throw new NotImplementedException();

    public AlphadigiLinhasDisplayAtualizadasEvent(int alphadigiId, string alphadigiNome, int novasLinhasDisplay)
    {
        AlphadigiId = alphadigiId;
        AlphadigiNome = alphadigiNome;
        NovasLinhasDisplay = novasLinhasDisplay;
    }
}