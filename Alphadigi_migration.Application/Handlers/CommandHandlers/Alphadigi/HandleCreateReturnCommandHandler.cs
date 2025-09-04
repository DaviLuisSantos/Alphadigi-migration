using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class HandleCreateReturnCommandHandler : IRequestHandler<HandleCreateReturnCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<HandleCreateReturnCommandHandler> _logger;

    public HandleCreateReturnCommandHandler(
        IMediator mediator,
        ILogger<HandleCreateReturnCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(HandleCreateReturnCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"HandleCreateReturn chamado com IP: {request.Ip}");

        try
        {
            // Buscar ou criar Alphadigi
            var getOrCreateQuery = new GetOrCreateAlphadigiQuery { Ip = request.Ip };
            var alphadigi = await _mediator.Send(getOrCreateQuery, cancellationToken);

            // Atualizar Alphadigi
            alphadigi.MarcarComoEnviado();

            var updateCommand = new UpdateAlphadigiEntityCommand { Alphadigi = alphadigi };
            await _mediator.Send(updateCommand, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em HandleCreateReturn");
            return false;
        }
    }
}
