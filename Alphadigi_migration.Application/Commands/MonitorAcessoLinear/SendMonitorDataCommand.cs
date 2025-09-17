
using Alphadigi_migration.Domain.DTOs.Veiculos;
using MediatR;

namespace Alphadigi_migration.Application.Commands.MonitorAcessoLinear;

public class SendMonitorDataCommand : IRequest<bool>
{
    public DadosVeiculoMonitorDTO Dados { get; set; }

   

    public SendMonitorDataCommand(DadosVeiculoMonitorDTO dados)
    {
        Dados = dados;
    }
}