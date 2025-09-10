using MediatR;


namespace Alphadigi_migration.Application.Queries.Veiculo;

public class PrepareVeiculoDataStringQuery : IRequest<string>
{
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
}
