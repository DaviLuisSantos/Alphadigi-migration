using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class CondominioCriadoEvent : DomainEvent
    {
        public override string EventType => "CondominioCriado";
        public override Guid AggregateId => CondominioId;

        public Guid CondominioId { get; }
        public string Nome { get; }
        public string Cnpj { get; }

        public CondominioCriadoEvent(Guid condominioId, string nome, string cnpj)
        {
            CondominioId = condominioId;
            Nome = nome;
            Cnpj = cnpj;
        }
    }

    public class CondominioAtualizadoEvent : DomainEvent
    {
        public override string EventType => "CondominioAtualizado";
        public override Guid AggregateId => CondominioId;

        public Guid CondominioId { get; }
        public string Nome { get; }

        public CondominioAtualizadoEvent(Guid condominioId, string nome)
        {
            CondominioId = condominioId;
            Nome = nome;
        }
    }

    public class CondominioCnpjAtualizadoEvent : DomainEvent
    {
        public override string EventType => "CondominioCnpjAtualizado";
        public override Guid AggregateId => CondominioId;

        public Guid CondominioId { get; }
        public string Nome { get; }
        public string Cnpj { get; }

        public CondominioCnpjAtualizadoEvent(Guid condominioId, string nome, string cnpj)
        {
            CondominioId = condominioId;
            Nome = nome;
            Cnpj = cnpj;
        }
    }

    public class CondominioAtivadoEvent : DomainEvent
    {
        public override string EventType => "CondominioAtivado";
        public override Guid AggregateId => CondominioId;

        public Guid CondominioId { get; }
        public string Nome { get; }

        public CondominioAtivadoEvent(Guid condominioId, string nome)
        {
            CondominioId = condominioId;
            Nome = nome;
        }
    }

    public class CondominioDesativadoEvent : DomainEvent
    {
        public override string EventType => "CondominioDesativado";
        public override Guid AggregateId => CondominioId;

        public Guid CondominioId { get; }
        public string Nome { get; }
        public string Motivo { get; }

        public CondominioDesativadoEvent(Guid condominioId, string nome, string motivo)
        {
            CondominioId = condominioId;
            Nome = nome;
            Motivo = motivo;
        }
    }
}