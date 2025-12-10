using MediatR;


namespace Alphadigi_migration.Application.Queries.Veiculo;

public class GetVeiculoByPlateQuery : IRequest<Domain.EntitiesNew.Veiculo>
{
    public string Plate { get; set; }
   
}
