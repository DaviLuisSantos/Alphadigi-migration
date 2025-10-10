//using Alphadigi_migration.Domain.Common;

//public class VisitanteEntradaRegistradaEvent : DomainEvent
//{
//    public string Placa { get; }
//    public string UnidadeDestino { get; }

//    public override string EventType => throw new NotImplementedException();

//    public override int AggregateId => throw new NotImplementedException();

//    // CORREÇÃO: Construtor recebe o ID e passa para a base
//    public VisitanteEntradaRegistradaEvent(int visitanteId, string placa, string unidadeDestino)
//        : base(visitanteId) // ← AQUI ESTAVA O ERRO: faltava passar o ID para a classe base
//    {
//        Placa = placa;
//        UnidadeDestino = unidadeDestino;
//    }
//}