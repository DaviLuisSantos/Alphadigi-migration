using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

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

    public async Task<object> Handle(ProcessHeartbeatCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"💓 HEARTBEAT recebido - IP: {request.Ip}");

        try
        {
            // 1. Buscar ou criar Alphadigi
            var getOrCreateQuery = new GetOrCreateAlphadigiQuery { Ip = request.Ip };
            var alphadigi = await _mediator.Send(getOrCreateQuery, cancellationToken);

            _logger.LogInformation($"📷 Câmera: {alphadigi.Ip}, Estado: {alphadigi.Estado}");

            // 2. Processar estágio do Alphadigi
            var stageCommand = new HandleAlphadigiStageCommand { Alphadigi = alphadigi };
            var stageResponse = await _mediator.Send(stageCommand, cancellationToken);

            // 3. Se stageResponse não for null, retornar
            if (stageResponse != null)
            {
                _logger.LogInformation($"📤 Retornando resposta do estágio: {alphadigi.Estado}");
                return stageResponse;
            }

            // 4. Heartbeat: NÃO ENVIA NADA para o display!
            // Apenas mantém a conexão

            var response = new ResponseHeathbeatDTO
            {
                Response_Heartbeat = new Response_AlarmInfoPlate
                {
                    info = "no",
                    serialData = new List<SerialData>()  // Lista VAZIA
                }
            };

            _logger.LogInformation("✅ Heartbeat: apenas verificação, sem envio para display");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERRO em ProcessHeartbeatCommandHandler");
            throw;
        }
    }
}