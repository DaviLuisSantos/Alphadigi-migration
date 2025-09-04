using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;

public class SendMonitorAcessoLinearCommandHandler : IRequestHandler<SendMonitorAcessoLinearCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendMonitorAcessoLinearCommandHandler> _logger;

    public SendMonitorAcessoLinearCommandHandler(IMediator mediator,
                                                 ILogger<SendMonitorAcessoLinearCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(SendMonitorAcessoLinearCommand request,
                             CancellationToken cancellationToken)
    {
        try
        {
            var monitorQuery = new SendDadosVeiculoMonitorQuery
            {
                Veiculo = request.Veiculo,
                Ip = request.IpCamera,
                Acesso = request.Acesso,
                HoraAcesso = request.Timestamp
            };

            return await _mediator.Send(monitorQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar dados para monitor de acesso linear");
            return false;
        }
    }
}
