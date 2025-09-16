using Alphadigi_migration.Domain.DTOs.Veiculos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Veiculo;

public class SendMonitorAcessoLinearCommand : IRequest<bool>
{
    public DadosVeiculoMonitorDTO DadosVeiculo { get; set; }
    public string IpCamera { get; set; }
    public string Acesso { get; set; }
    public DateTime Timestamp { get; set; }
}
