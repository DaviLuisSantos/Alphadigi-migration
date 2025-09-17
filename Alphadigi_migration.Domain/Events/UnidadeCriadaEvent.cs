

using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events;

public class UnidadeCriadaEvent : DomainEvent
{
    public int UnidadeId { get; }
    public string Nome { get; }
    public int NumeroVagas { get; }

    public override string EventType => throw new NotImplementedException();

    public override int AggregateId => throw new NotImplementedException();

    public UnidadeCriadaEvent(int unidadeId, string nome, int numeroVagas)
    {
        UnidadeId = unidadeId;
        Nome = nome;
        NumeroVagas = numeroVagas;
    }
}