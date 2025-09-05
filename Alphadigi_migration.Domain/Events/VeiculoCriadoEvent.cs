using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class VeiculoCriadoEvent : DomainEvent
    {
        public override string EventType => "VeiculoCriado";
        public override int AggregateId => VeiculoId;

        public int VeiculoId { get; }
        public string Placa { get; }
        public string Unidade { get; }

        public VeiculoCriadoEvent(int veiculoId, string placa, string unidade)
        {
            VeiculoId = veiculoId;
            Placa = placa;
            Unidade = unidade;
        }
    }

    public class VeiculoAtualizadoEvent : DomainEvent
    {
        public override string EventType => "VeiculoAtualizado";
        public override int AggregateId => VeiculoId;

        public int VeiculoId { get; }
        public string Placa { get; }

        public VeiculoAtualizadoEvent(int veiculoId, string placa)
        {
            VeiculoId = veiculoId;
            Placa = placa;
        }
    }

    public class VeiculoAcessoRegistradoEvent : DomainEvent
    {
        public override string EventType => "VeiculoAcessoRegistrado";
        public override int AggregateId => VeiculoId;

        public int VeiculoId { get; }
        public string Placa { get; }
        public string IpCamera { get; }
        public DateTime DataHoraAcesso { get; }

        public VeiculoAcessoRegistradoEvent(int veiculoId, string placa, string ipCamera, DateTime dataHoraAcesso)
        {
            VeiculoId = veiculoId;
            Placa = placa;
            IpCamera = ipCamera;
            DataHoraAcesso = dataHoraAcesso;
        }
    }

    public class VeiculoEntrouEvent : DomainEvent
    {
        public override string EventType => "VeiculoEntrou";
        public override int AggregateId => VeiculoId;

        public int VeiculoId { get; }
        public string Placa { get; }
        public string IpCamera { get; }

        public VeiculoEntrouEvent(int veiculoId, string placa, string ipCamera)
        {
            VeiculoId = veiculoId;
            Placa = placa;
            IpCamera = ipCamera;
        }
    }

    public class VeiculoSaiuEvent : DomainEvent
    {
        public override string EventType => "VeiculoSaiu";
        public override int AggregateId => VeiculoId;

        public int VeiculoId { get; }
        public string Placa { get; }
        public string IpCamera { get; }

        public VeiculoSaiuEvent(int veiculoId, string placa, string ipCamera)
        {
            VeiculoId = veiculoId;
            Placa = placa;
            IpCamera = ipCamera;
        }
    }

    public class VeiculoRotaAtualizadaEvent : DomainEvent
    {
        public override string EventType => "VeiculoRotaAtualizada";
        public override int AggregateId => VeiculoId;

        public int VeiculoId { get; }
        public string Placa { get; }
        public int? IdRota { get; }

        public VeiculoRotaAtualizadaEvent(int veiculoId, string placa, int? idRota)
        {
            VeiculoId = veiculoId;
            Placa = placa;
            IdRota = idRota;
        }
    }
}