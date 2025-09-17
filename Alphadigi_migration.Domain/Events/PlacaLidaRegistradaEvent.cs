// Alphadigi_migration.Domain/Events/PlacaLidaEvents.cs
using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class PlacaLidaRegistradaEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaRegistrada";
        public override int AggregateId => PlacaLidaId;

        public int PlacaLidaId { get; }
        public string Placa { get; }
        public int AlphadigiId { get; }
        public DateTime DataHora { get; }

        public PlacaLidaRegistradaEvent(int placaLidaId, string placa, int alphadigiId, DateTime dataHora)
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
        public override int AggregateId => PlacaLidaId;

        public int PlacaLidaId { get; }
        public string Placa { get; }
        public bool Liberado { get; }
        public string Acesso { get; }

        public PlacaLidaProcessadaEvent(int placaLidaId, string placa, bool liberado, string acesso)
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
        public override int AggregateId => PlacaLidaId;

        public int PlacaLidaId { get; }
        public string Placa { get; }
        public bool Cadastrado { get; }

        public PlacaLidaCadastroAtualizadoEvent(int placaLidaId, string placa, bool cadastrado)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
            Cadastrado = cadastrado;
        }
    }

    public class PlacaLidaImagensAtualizadasEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaImagensAtualizadas";
        public override int AggregateId => PlacaLidaId;

        public int PlacaLidaId { get; }
        public string Placa { get; }

        public PlacaLidaImagensAtualizadasEvent(int placaLidaId, string placa)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
        }
    }

    public class PlacaLidaMarcadaComoNaoRealEvent : DomainEvent
    {
        public override string EventType => "PlacaLidaMarcadaComoNaoReal";
        public override int AggregateId => PlacaLidaId;

        public int PlacaLidaId { get; }
        public string Placa { get; }

        public PlacaLidaMarcadaComoNaoRealEvent(int placaLidaId, string placa)
        {
            PlacaLidaId = placaLidaId;
            Placa = placa;
        }
    }
}