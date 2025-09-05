using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.Events
{
    public class AcessoRegistradoEvent : DomainEvent
    {
        public override string EventType => "AcessoRegistrado";
        public override int AggregateId => AcessoId;

        public int AcessoId { get; }
        public string Placa { get; }
        public string Local { get; }
        public bool AcessoPermitido { get; }

        public AcessoRegistradoEvent(int acessoId, string placa, string local, bool acessoPermitido)
        {
            AcessoId = acessoId;
            Placa = placa;
            Local = local;
            AcessoPermitido = acessoPermitido;
        }
    }

    public class AcessoPermitidoEvent : DomainEvent
    {
        public override string EventType => "AcessoPermitido";
        public override int AggregateId => AcessoId;

        public int AcessoId { get; }
        public string Placa { get; }
        public string Local { get; }
        public string Motivo { get; }

        public AcessoPermitidoEvent(int acessoId, string placa, string local, string motivo)
        {
            AcessoId = acessoId;
            Placa = placa;
            Local = local;
            Motivo = motivo;
        }
    }

    public class AcessoNegadoEvent : DomainEvent
    {
        public override string EventType => "AcessoNegado";
        public override int AggregateId => AcessoId;

        public int AcessoId { get; }
        public string Placa { get; }
        public string Local { get; }
        public string Motivo { get; }

        public AcessoNegadoEvent(int acessoId, string placa, string local, string motivo)
        {
            AcessoId = acessoId;
            Placa = placa;
            Local = local;
            Motivo = motivo;
        }
    }

    public class AcessoFotoAtualizadaEvent : DomainEvent
    {
        public override string EventType => "AcessoFotoAtualizada";
        public override int AggregateId => AcessoId;

        public int AcessoId { get; }
        public string Placa { get; }
        public string FotoUrl { get; }

        public AcessoFotoAtualizadaEvent(int acessoId, string placa, string fotoUrl)
        {
            AcessoId = acessoId;
            Placa = placa;
            FotoUrl = fotoUrl;
        }
    }

    public class AcessoDadosVeiculoAtualizadosEvent : DomainEvent
    {
        public override string EventType => "AcessoDadosVeiculoAtualizados";
        public override int AggregateId => AcessoId;

        public int AcessoId { get; }
        public string Placa { get; }
        public string DadosVeiculo { get; }

        public AcessoDadosVeiculoAtualizadosEvent(int acessoId, string placa, string dadosVeiculo)
        {
            AcessoId = acessoId;
            Placa = placa;
            DadosVeiculo = dadosVeiculo;
        }
    }
}