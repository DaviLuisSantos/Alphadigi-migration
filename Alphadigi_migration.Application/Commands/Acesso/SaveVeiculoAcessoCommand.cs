using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Acesso;

public class SaveVeiculoAcessoCommand : IRequest<bool>
{
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public DateTime Timestamp { get; set; }
    public string Imagem { get; set; }
    public bool AcessoPermitido { get; set; }
    public string MotivoAcesso { get; set; }
    public bool IsVisitante { get; set; } = false;
}