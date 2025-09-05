using MediatR;


namespace Alphadigi_migration.Application.Commands.Acesso;

public class HandleAccessCommand : IRequest<(bool ShouldReturn, string Acesso)>
{
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    public Domain.EntitiesNew.Area Area { get; set; }
}
