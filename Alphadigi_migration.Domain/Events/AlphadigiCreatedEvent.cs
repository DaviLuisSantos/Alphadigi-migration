// Alphadigi_migration.Domain/Events/AlphadigiEvents.cs
using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class AlphadigiCreatedEvent : DomainEvent
    {
        public override string EventType => "AlphadigiCreated";

        public int AlphadigiId { get; }
        public string Nome { get; }
        public string Ip { get; }
        public int AreaId { get; }

        public override int AggregateId => throw new NotImplementedException();

        public AlphadigiCreatedEvent(int alphadigiId, string nome, string ip, int areaId)
        {
            AlphadigiId = alphadigiId;
            Nome = nome;
            Ip = ip;
            AreaId = areaId;
        }
    }

    public class AlphadigiUpdatedEvent : DomainEvent
    {
        public override string EventType => "AlphadigiUpdated";

        public int AlphadigiId { get; }
        public string Descricao { get; }

        public override int AggregateId => throw new NotImplementedException();

        public AlphadigiUpdatedEvent(int alphadigiId, string descricao)
        {
            AlphadigiId = alphadigiId;
            Descricao = descricao;
        }
    }

    public class AlphadigiAtivadoEvent : DomainEvent
    {
        public override string EventType => "AlphadigiAtivado";

        public int AlphadigiId { get; }
        public string Nome { get; }

        public override int AggregateId => throw new NotImplementedException();

        public AlphadigiAtivadoEvent(int alphadigiId, string nome)
        {
            AlphadigiId = alphadigiId;
            Nome = nome;
        }
    }

    public class AlphadigiDesativadoEvent : DomainEvent
    {
        public override string EventType => "AlphadigiDesativado";

        public int AlphadigiId { get; }
        public string Nome { get; }
        public string Motivo { get; }

        public override int AggregateId => throw new NotImplementedException();

        public AlphadigiDesativadoEvent(int alphadigiId, string nome, string motivo)
        {
            AlphadigiId = alphadigiId;
            Nome = nome;
            Motivo = motivo;
        }
    }

    public class PlacaProcessadaEvent : DomainEvent
    {
        public override string EventType => "PlacaProcessada";

        public int AlphadigiId { get; }
        public string Placa { get; }
        public int UltimoId { get; }

        public override int AggregateId => throw new NotImplementedException();

        public PlacaProcessadaEvent(int alphadigiId, string placa, int ultimoId)
        {
            AlphadigiId = alphadigiId;
            Placa = placa;
            UltimoId = ultimoId;
        }
    }

    public class DisplayConfiguradoEvent : DomainEvent
    {
        public override string EventType => "DisplayConfigurado";

        public int AlphadigiId { get; }
        public int LinhasDisplay { get; }

        public override int AggregateId => throw new NotImplementedException();

        public DisplayConfiguradoEvent(int alphadigiId, int linhasDisplay)
        {
            AlphadigiId = alphadigiId;
            LinhasDisplay = linhasDisplay;
        }
    }

    public class FotoEventoConfiguradoEvent : DomainEvent
    {
        public override string EventType => "FotoEventoConfigurado";

        public int AlphadigiId { get; }
        public bool Habilitado { get; }

        public override int AggregateId => throw new NotImplementedException();

        public FotoEventoConfiguradoEvent(int alphadigiId, bool habilitado)
        {
            AlphadigiId = alphadigiId;
            Habilitado = habilitado;
        }
    }
}