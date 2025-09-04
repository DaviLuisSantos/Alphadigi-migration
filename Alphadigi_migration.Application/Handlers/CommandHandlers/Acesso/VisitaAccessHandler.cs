using Alphadigi_migration.Application.Commands.Acesso;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class VisitaAccessHandler : IRequestHandler<HandleAccessCommand, (bool ShouldReturn, string Acesso)>
{
    private readonly ILogger<VisitaAccessHandler> _logger;

    public VisitaAccessHandler(ILogger<VisitaAccessHandler> logger)
    {
        _logger = logger;
    }

    public Task<(bool ShouldReturn, string Acesso)> Handle(HandleAccessCommand request, 
                                                        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processando acesso de visitante para veículo com placa {request.Veiculo?.Placa ?? "Visitante"}.");
        return Task.FromResult((false, "NÃO CADASTRADO"));
    }

}
