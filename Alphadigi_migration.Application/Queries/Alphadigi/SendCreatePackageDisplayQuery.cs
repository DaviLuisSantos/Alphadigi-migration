using MediatR;


namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class SendCreatePackageDisplayQuery : IRequest<List<Domain.DTOs.Alphadigi.SerialData>>
{
    public string  Placa { get; set; }
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public string Acesso { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
}
