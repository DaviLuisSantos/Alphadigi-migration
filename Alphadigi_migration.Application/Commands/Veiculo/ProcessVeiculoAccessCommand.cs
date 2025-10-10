using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Veiculo;

public class ProcessVeiculoAccessCommand : IRequest<(bool ShouldReturn, string Acesso)>
{
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    public DateTime Timestamp { get; set; }

    public bool IsVisitante { get; set; } = false;
}
