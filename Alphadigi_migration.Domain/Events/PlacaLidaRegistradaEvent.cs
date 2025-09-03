// Alphadigi_migration.Domain/Events/PlacaLidaEvents.cs
using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class PlacaLidaRegistradaEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaRegistrada";
        public override Guid AggregateId => PlacaLidaId;

        public Guid PlacaLidaId { get; }
        public string Placa { get; }
        public Guid AlphadigiId { get; }
        public DateTime DataHora { get; }

        public PlacaLidaRegistradaEvent(Guid placaLidaId, string placa, Guid alphadigiId, DateTime dataHora)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
            AlphadigiId = alphadigiId;
            DataHora = dataHora;
        }
    }

    public class PlacaLidaProcessadaEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaProcessada";
        public override Guid AggregateId => PlacaLidaId;

        public Guid PlacaLidaId { get; }
        public string Placa { get; }
        public bool Liberado { get; }
        public string Acesso { get; }

        public PlacaLidaProcessadaEvent(Guid placaLidaId, string placa, bool liberado, string acesso)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
            Liberado = liberado;
            Acesso = acesso;
        }
    }

    public class PlacaLidaCadastroAtualizadoEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaCadastroAtualizado";
        public override Guid AggregateId => PlacaLidaId;

        public Guid PlacaLidaId { get; }
        public string Placa { get; }
        public bool Cadastrado { get; }

        public PlacaLidaCadastroAtualizadoEvent(Guid placaLidaId, string placa, bool cadastrado)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
            Cadastrado = cadastrado;
        }
    }

    public class PlacaLidaImagensAtualizadasEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaImagensAtualizadas";
        public override Guid AggregateId => PlacaLidaId;

        public Guid PlacaLidaId { get; }
        public string Placa { get; }

        public PlacaLidaImagensAtualizadasEvent(Guid placaLidaId, string placa)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
        }
    }

    public class PlacaLidaMarcadaComoNaoRealEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaMarcadaComoNaoReal";
        public override Guid AggregateId => PlacaLidaId;

        public Guid PlacaLidaId { get; }
        public string Placa { get; }

        public PlacaLidaMarcadaComoNaoRealEvent(Guid placaLidaId, string placa)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
        }
    }
}