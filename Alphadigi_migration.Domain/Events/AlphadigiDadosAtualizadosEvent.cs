using Alphadigi_migration.Domain.Common;


namespace Alphadigi_migration.Domain.Events;


public class AlphadigiDadosAtualizadosEvent : DomainEvent
{
    public int AlphadigiId { get; }
    public int NovaAreaId { get; }
    public bool NovoSentido { get; }

    public override string EventType => throw new NotImplementedException();

    public override Guid AggregateId => throw new NotImplementedException();

    public AlphadigiDadosAtualizadosEvent(int alphadigiId, int novaAreaId, bool novoSentido)
    {
        AlphadigiId = alphadigiId;
        NovaAreaId = novaAreaId;
        NovoSentido = novoSentido;
    }
}