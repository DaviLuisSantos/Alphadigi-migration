using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Queries.Area;
using Alphadigi_migration.Application.Service;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class ProcessAccessCommandHandler : IRequestHandler<HandleAccessCommand, (bool ShouldReturn, string Acesso)>
{
    private readonly IMediator _mediator;
    private readonly IAccessHandlerFactory _handlerFactory;
    private readonly ILogger<ProcessAccessCommandHandler> _logger;

    public ProcessAccessCommandHandler(
        IMediator mediator,
        IAccessHandlerFactory handlerFactory,
        ILogger<ProcessAccessCommandHandler> logger)
    {
        _mediator = mediator;
        _handlerFactory = handlerFactory;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(HandleAccessCommand request, 
                                                                CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processando acesso para veículo: {request.Veiculo?.Placa}");

        try
        {
            // Se não tem área definida, buscar da câmera
            if (request.Area == null && request.Alphadigi?.AreaId !=  null)
            {
                var areaQuery = new GetAreaByIdQuery { id = request.Alphadigi.AreaId };
                request.Area = await _mediator.Send(areaQuery, cancellationToken);
            }

            // Selecionar handler baseado na área
            var handler = _handlerFactory.GetAccessHandler(request.Area);

            // Executar handler específico
            return await handler.Handle(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao processar acesso para veículo {request.Veiculo?.Placa}");
            return (false, "ERRO NO PROCESSAMENTO");
        }
    }
}