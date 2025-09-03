using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class CameraCriadaEvent : DomainEvent
    {
        public override string EventType => "CameraCriada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string Nome { get; }
        public string Ip { get; }
        public Guid AreaId { get; }

        public CameraCriadaEvent(Guid cameraId, string nome, string ip, Guid areaId)
        {
            CameraId = cameraId;
            Nome = nome;
            Ip = ip;
            AreaId = areaId;
        }
    }

    public class CameraAtualizadaEvent : DomainEvent
    {
        public override string EventType => "CameraAtualizada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string Nome { get; }
        public string Descricao { get; }

        public CameraAtualizadaEvent(Guid cameraId, string nome, string descricao)
        {
            CameraId = cameraId;
            Nome = nome;
            Descricao = descricao;
        }
    }

    public class CameraAreaAtualizadaEvent : DomainEvent
    {
        public override string EventType => "CameraAreaAtualizada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string Nome { get; }
        public Guid NovaAreaId { get; }

        public CameraAreaAtualizadaEvent(Guid cameraId, string nome, Guid novaAreaId)
        {
            CameraId = cameraId;
            Nome = nome;
            NovaAreaId = novaAreaId;
        }
    }

    public class CameraAreaAssociadaEvent : DomainEvent
    {
        public override string EventType => "CameraAreaAssociada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string NomeCamera { get; }
        public Guid AreaId { get; }
        public string NomeArea { get; }

        public CameraAreaAssociadaEvent(Guid cameraId, string nomeCamera, Guid areaId, string nomeArea)
        {
            CameraId = cameraId;
            NomeCamera = nomeCamera;
            AreaId = areaId;
            NomeArea = nomeArea;
        }
    }

    public class CameraFotoEventoConfiguradaEvent : DomainEvent
    {
        public override string EventType => "CameraFotoEventoConfigurada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string Nome { get; }
        public bool Habilitada { get; }

        public CameraFotoEventoConfiguradaEvent(Guid cameraId, string nome, bool habilitada)
        {
            CameraId = cameraId;
            Nome = nome;
            Habilitada = habilitada;
        }
    }

    public class CameraAtivadaEvent : DomainEvent
    {
        public override string EventType => "CameraAtivada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string Nome { get; }

        public CameraAtivadaEvent(Guid cameraId, string nome)
        {
            CameraId = cameraId;
            Nome = nome;
        }
    }

    public class CameraDesativadaEvent : DomainEvent
    {
        public override string EventType => "CameraDesativada";
        public override Guid AggregateId => CameraId;

        public Guid CameraId { get; }
        public string Nome { get; }
        public string Motivo { get; }

        public CameraDesativadaEvent(Guid cameraId, string nome, string motivo)
        {
            CameraId = cameraId;
            Nome = nome;
            Motivo = motivo;
        }
    }
}