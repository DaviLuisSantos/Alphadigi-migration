//using Alphadigi_migration.Domain.Common;

//public class VisitanteValidadoEvent : DomainEvent
//{
//    public string Placa { get; }
//    public bool AcessoPermitido { get; }
//    public string Mensagem { get; }

//    public override string EventType => throw new NotImplementedException();

//    public override int AggregateId => throw new NotImplementedException();

//    public VisitanteValidadoEvent(int visitanteId, string placa, bool acessoPermitido, string mensagem)
//        : base(visitanteId) // ← AQUI ESTAVA O ERRO: faltava passar o ID para a classe base
//    {
//        Placa = placa;
//        AcessoPermitido = acessoPermitido;
//        Mensagem = mensagem;
//    }
//}
