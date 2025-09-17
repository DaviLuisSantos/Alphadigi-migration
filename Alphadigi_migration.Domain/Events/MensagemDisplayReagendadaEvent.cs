using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class MensagemDisplayCriadaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayCriada";
        public override int AggregateId => MensagemId;

        public int MensagemId { get; }
        public string Placa { get; }
        public string Mensagem { get; }
        public int AlphadigiId { get; }

        public MensagemDisplayCriadaEvent(int mensagemId, string placa, string mensagem, int alphadigiId)
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
        public override int AggregateId => MensagemId;

        public int MensagemId { get; }
        public string Placa { get; }
        public string NovaMensagem { get; }

        public MensagemDisplayAtualizadaEvent(int mensagemId, string placa, string novaMensagem)
        {
            MensagemId = mensagemId;
            Placa = placa;
            NovaMensagem = novaMensagem;
        }
    }

    public class MensagemDisplayExibidaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayExibida";
        public override int AggregateId => MensagemId;

        public int MensagemId { get; }
        public string Placa { get; }
        public string Mensagem { get; }

        public MensagemDisplayExibidaEvent(int mensagemId, string placa, string mensagem)
        {
            MensagemId = mensagemId;
            Placa = placa;
            Mensagem = mensagem;
        }
    }

    public class MensagemDisplayPrioridadeAlteradaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayPrioridadeAlterada";
        public override int AggregateId => MensagemId;

        public int MensagemId { get; }
        public string Placa { get; }
        public int NovaPrioridade { get; }

        public MensagemDisplayPrioridadeAlteradaEvent(int mensagemId, string placa, int novaPrioridade)
        {
            MensagemId = mensagemId;
            Placa = placa;
            NovaPrioridade = novaPrioridade;
        }
    }

    public class MensagemDisplayReagendadaEvent : DomainEvent
    {
        public override string EventType => "MensagemDisplayReagendada";
        public override int AggregateId => MensagemId;

        public int MensagemId { get; }
        public string Placa { get; }
        public DateTime NovaDataHora { get; }

        public MensagemDisplayReagendadaEvent(int mensagemId, string placa, DateTime novaDataHora)
        {
            MensagemId = mensagemId;
            Placa = placa;
            NovaDataHora = novaDataHora;
        }
    }
}