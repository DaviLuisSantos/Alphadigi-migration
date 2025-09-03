using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class MensagemDisplayCriadaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayCriada";
        public override Guid AggregateId => MensagemId;

        public Guid MensagemId { get; }
        public string Placa { get; }
        public string Mensagem { get; }
        public Guid AlphadigiId { get; }

        public MensagemDisplayCriadaEvent(Guid mensagemId, string placa, string mensagem, Guid alphadigiId)
        {
            MensagemId = mensagemId;
            Placa = placa;
            Mensagem = mensagem;
            AlphadigiId = alphadigiId;
        }
    }

    public class MensagemDisplayAtualizadaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayAtualizada";
        public override Guid AggregateId => MensagemId;

        public Guid MensagemId { get; }
        public string Placa { get; }
        public string NovaMensagem { get; }

        public MensagemDisplayAtualizadaEvent(Guid mensagemId, string placa, string novaMensagem)
        {
            MensagemId = mensagemId;
            Placa = placa;
            NovaMensagem = novaMensagem;
        }
    }

    public class MensagemDisplayExibidaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayExibida";
        public override Guid AggregateId => MensagemId;

        public Guid MensagemId { get; }
        public string Placa { get; }
        public string Mensagem { get; }

        public MensagemDisplayExibidaEvent(Guid mensagemId, string placa, string mensagem)
        {
            MensagemId = mensagemId;
            Placa = placa;
            Mensagem = mensagem;
        }
    }

    public class MensagemDisplayPrioridadeAlteradaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayPrioridadeAlterada";
        public override Guid AggregateId => MensagemId;

        public Guid MensagemId { get; }
        public string Placa { get; }
        public int NovaPrioridade { get; }

        public MensagemDisplayPrioridadeAlteradaEvent(Guid mensagemId, string placa, int novaPrioridade)
        {
            MensagemId = mensagemId;
            Placa = placa;
            NovaPrioridade = novaPrioridade;
        }
    }

    public class MensagemDisplayReagendadaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayReagendada";
        public override Guid AggregateId => MensagemId;

        public Guid MensagemId { get; }
        public string Placa { get; }
        public DateTime NovaDataHora { get; }

        public MensagemDisplayReagendadaEvent(Guid mensagemId, string placa, DateTime novaDataHora)
        {
            MensagemId = mensagemId;
            Placa = placa;
            NovaDataHora = novaDataHora;
        }
    }
}