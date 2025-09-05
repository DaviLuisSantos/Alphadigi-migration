

using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events;

public class AlphadigiEstadoAtualizadoEvent : DomainEvent
{
    public int AlphadigiId { get; }
    public string NovoEstado { get; }
    public string EstadoAnterior { get; }
    public DateTime DataAtualizacao { get; }

    public override string EventType => throw new NotImplementedException();

    public override Guid AggregateId => throw new NotImplementedException();

    public AlphadigiEstadoAtualizadoEvent(int alphadigiId, string novoEstado, string estadoAnterior = null)
    {
        AlphadigiId = alphadigiId;
        NovoEstado = novoEstado;
        EstadoAnterior = estadoAnterior;
        DataAtualizacao = DateTime.UtcNow;
    }
}
