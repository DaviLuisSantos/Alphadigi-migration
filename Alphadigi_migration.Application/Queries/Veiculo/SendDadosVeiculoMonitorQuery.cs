using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Veiculo;

public class SendDadosVeiculoMonitorQuery : IRequest<bool>
{
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public string Ip { get; set; }
    public string Acesso { get; set; }
    public DateTime HoraAcesso { get; set; }
}
