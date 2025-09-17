using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class CameraCriadaEvent : DomainEvent
    {
        public override string EventType => "CameraCriada";
        public override int AggregateId => CameraId;

        public int CameraId { get; }
        public string Nome { get; }
        public string Ip { get; }
        public int AreaId { get; }

        public CameraCriadaEvent(int cameraId, string nome, string ip, int areaId)
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
        public override int AggregateId => CameraId;

        public  int CameraId { get; }
        public string Nome { get; }
        public string Descricao { get; }

        public CameraAtualizadaEvent(int cameraId, string nome, string descricao)
        {
            CameraId = cameraId;
            Nome = nome;
            Descricao = descricao;
        }
    }

    public class CameraAreaAtualizadaEvent : DomainEvent
    {
        public override string EventType => "CameraAreaAtualizada";
        public override int AggregateId => CameraId;

        public int CameraId { get; }
        public string Nome { get; }
        public int NovaAreaId { get; }

        public CameraAreaAtualizadaEvent(int cameraId, string nome, int novaAreaId)
        {
            CameraId = cameraId;
            Nome = nome;
            NovaAreaId = novaAreaId;
        }
    }

    public class CameraAreaAssociadaEvent : DomainEvent
    {
        public override string EventType => "CameraAreaAssociada";
        public override int AggregateId => CameraId;

        public int CameraId { get; }
        public string NomeCamera { get; }
        public int AreaId { get; }
        public string NomeArea { get; }

        public CameraAreaAssociadaEvent(int cameraId, string nomeCamera, int areaId, string nomeArea)
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
        public override int AggregateId => CameraId;

        public int CameraId { get; }
        public string Nome { get; }
        public bool Habilitada { get; }

        public CameraFotoEventoConfiguradaEvent(int cameraId, string nome, bool habilitada)
        {
            CameraId = cameraId;
            Nome = nome;
            Habilitada = habilitada;
        }
    }

    public class CameraAtivadaEvent : DomainEvent
    {
        public override string EventType => "CameraAtivada";
        public override int AggregateId => CameraId;

        public int CameraId { get; }
        public string Nome { get; }

        public CameraAtivadaEvent(int cameraId, string nome)
        {
            CameraId = cameraId;
            Nome = nome;
        }
    }

    public class CameraDesativadaEvent : DomainEvent
    {
        public override string EventType => "CameraDesativada";
        public override int AggregateId => CameraId;

        public int CameraId { get; }
        public string Nome { get; }
        public string Motivo { get; }

        public CameraDesativadaEvent(int cameraId, string nome, string motivo)
        {
            CameraId = cameraId;
            Nome = nome;
            Motivo = motivo;
        }
    }
}