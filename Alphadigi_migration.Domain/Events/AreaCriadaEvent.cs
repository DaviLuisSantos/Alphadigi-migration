using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class AreaCriadaEvent : DomainEvent
    {
        public override string EventType => "AreaCriada";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }

        public AreaCriadaEvent(Guid areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaAtualizadaEvent : DomainEvent
    {
        public override string EventType => "AreaAtualizada";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }

        public AreaAtualizadaEvent(Guid areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaControleVagaAtivadoEvent : DomainEvent
    {
        public override string EventType => "AreaControleVagaAtivado";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }

        public AreaControleVagaAtivadoEvent(Guid areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaControleVagaDesativadoEvent : DomainEvent
    {
        public override string EventType => "AreaControleVagaDesativado";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }

        public AreaControleVagaDesativadoEvent(Guid areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaTempoAntipassbackConfiguradoEvent : DomainEvent
    {
        public override string EventType => "AreaTempoAntipassbackConfigurado";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }
        public TimeSpan TempoAntipassback { get; }

        public AreaTempoAntipassbackConfiguradoEvent(Guid areaId, string nome, TimeSpan tempoAntipassback)
        {
            AreaId = areaId;
            Nome = nome;
            TempoAntipassback = tempoAntipassback;
        }
    }

    public class AreaTempoAntipassbackRemovidoEvent : DomainEvent
    {
        public override string EventType => "AreaTempoAntipassbackRemovido";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }

        public AreaTempoAntipassbackRemovidoEvent(Guid areaId, string nome)
        {
            AreaId = areaId;
            Nome = nome;
        }
    }

    public class AreaAcessoVisitantesConfiguradoEvent : DomainEvent
    {
        public override string EventType => "AreaAcessoVisitantesConfigurado";
        public override Guid AggregateId => AreaId;

        public Guid AreaId { get; }
        public string Nome { get; }
        public bool EntradaHabilitada { get; }
        public bool SaidaHabilitada { get; }

        public AreaAcessoVisitantesConfiguradoEvent(Guid areaId, string nome, bool entradaHabilitada, bool saidaHabilitada)
        {
            AreaId = areaId;
            Nome = nome;
            EntradaHabilitada = entradaHabilitada;
            SaidaHabilitada = saidaHabilitada;
        }
    }
}