using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class CondominioCriadoEvent : DomainEvent
    {
        public override string EventType => "CondominioCriado";
        public override int AggregateId => CondominioId;

        public int CondominioId { get; }
        public string Nome { get; }
        public string Cnpj { get; }

        public CondominioCriadoEvent(int condominioId, string nome, string cnpj)
        {
            CondominioId = condominioId;
            Nome = nome;
            Cnpj = cnpj;
        }
    }

    public class CondominioAtualizadoEvent : DomainEvent
    {
        public override string EventType => "CondominioAtualizado";
        public override int AggregateId => CondominioId;

        public int CondominioId { get; }
        public string Nome { get; }

        public CondominioAtualizadoEvent(int condominioId, string nome)
        {
            CondominioId = condominioId;
            Nome = nome;
        }
    }

    public class CondominioCnpjAtualizadoEvent : DomainEvent
    {
        public override string EventType => "CondominioCnpjAtualizado";
        public override int AggregateId => CondominioId;

        public int CondominioId { get; }
        public string Nome { get; }
        public string Cnpj { get; }

        public CondominioCnpjAtualizadoEvent(int condominioId, string nome, string cnpj)
        {
            CondominioId = condominioId;
            Nome = nome;
            Cnpj = cnpj;
        }
    }

    public class CondominioAtivadoEvent : DomainEvent
    {
        public override string EventType => "CondominioAtivado";
        public override int AggregateId => CondominioId;

        public int CondominioId { get; }
        public string Nome { get; }

        public CondominioAtivadoEvent(int condominioId, string nome)
        {
            CondominioId = condominioId;
            Nome = nome;
        }
    }

    public class CondominioDesativadoEvent : DomainEvent
    {
        public override string EventType => "CondominioDesativado";
        public override int AggregateId => CondominioId;

        public int CondominioId { get; }
        public string Nome { get; }
        public string Motivo { get; }

        public CondominioDesativadoEvent(int condominioId, string nome, string motivo)
        {
            CondominioId = condominioId;
            Nome = nome;
            Motivo = motivo;
        }
    }
}