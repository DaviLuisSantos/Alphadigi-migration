using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class ProcessHeartbeatCommandHandler : IRequestHandler<ProcessHeartbeatCommand, object>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessHeartbeatCommandHandler> _logger;

    public ProcessHeartbeatCommandHandler(
        IMediator mediator,
        ILogger<ProcessHeartbeatCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<object> Handle(ProcessHeartbeatCommand request, 
                                     CancellationToken cancellationToken)
    {
        _logger.LogInformation($"ProcessHeartbeat chamado com IP: {request.Ip}");

        try
        {
            // Buscar ou criar Alphadigi usando query
            var getOrCreateQuery = new GetOrCreateAlphadigiQuery { Ip = request.Ip };
            var alphadigi = await _mediator.Send(getOrCreateQuery, cancellationToken);

            // Processar estágio do Alphadigi
            var stageCommand = new HandleAlphadigiStageCommand { Alphadigi = alphadigi };
            return await _mediator.Send(stageCommand, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em ProcessHeartbeat");
            throw;
        }
    }
}

