
using MediatR;

namespace Alphadigi_migration.Application.Commands.Display;

public class SendToDisplayCommand : IRequest
{
    public string Placa { get; set; }
    public string Acesso { get; set; }
    public bool Liberado { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public bool IsVisitante { get; set; } = false;
    public string VisitanteNome { get; set; }
    public string VisitanteUnidade { get; set; }


}