using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class AreaCriadaEvent : DomainEvent
    {
        public override string EventType => "AreaCriada";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }

        public AreaCriadaEvent(int areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaAtualizadaEvent : DomainEvent
    {
        public override string EventType => "AreaAtualizada";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }

        public AreaAtualizadaEvent(int areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaControleVagaAtivadoEvent : DomainEvent
    {
        public override string EventType => "AreaControleVagaAtivado";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }

        public AreaControleVagaAtivadoEvent(int areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaControleVagaDesativadoEvent : DomainEvent
    {
        public override string EventType => "AreaControleVagaDesativado";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }

        public AreaControleVagaDesativadoEvent(int areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaTempoAntipassbackConfiguradoEvent : DomainEvent
    {
        public override string EventType => "AreaTempoAntipassbackConfigurado";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }
        public string TempoAntipassback { get; }

        public AreaTempoAntipassbackConfiguradoEvent(int areaId, string nome, string tempoAntipassback)
        {
            AreaId = areaId;
            Nome = nome;
            TempoAntipassback = tempoAntipassback;
        }
    }

    public class AreaTempoAntipassbackRemovidoEvent : DomainEvent
    {
        public override string EventType => "AreaTempoAntipassbackRemovido";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }

        public AreaTempoAntipassbackRemovidoEvent(int areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaAcessoVisitantesConfiguradoEvent : DomainEvent
    {
        public override string EventType => "AreaAcessoVisitantesConfigurado";
        public override int AggregateId => AreaId;

        public int AreaId { get; }
        public string Nome { get; }
        public bool EntradaHabilitada { get; }
        public bool SaidaHabilitada { get; }

        public AreaAcessoVisitantesConfiguradoEvent(int areaId, string nome, bool entradaHabilitada, bool saidaHabilitada)
        {
            AreaId = areaId;
            Nome = nome;
            EntradaHabilitada = entradaHabilitada;
            SaidaHabilitada = saidaHabilitada;
        }
    }
}