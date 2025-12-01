using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class ProcessHeartbeatCommandHandler : IRequestHandler<ProcessHeartbeatCommand, object>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessHeartbeatCommandHandler> _logger;
    private readonly DisplayService _displayService;
    private readonly ICondominioRepository _condominioRepository;

    public ProcessHeartbeatCommandHandler(
        IMediator mediator,
        ILogger<ProcessHeartbeatCommandHandler> logger,
        DisplayService displayService,
        ICondominioRepository condominioRepository)
    {
        _mediator = mediator;
        _logger = logger;
        _displayService = displayService;
        _condominioRepository = condominioRepository;
    }

    public async Task<object> Handle(ProcessHeartbeatCommand request,
                                     CancellationToken cancellationToken)
    {
        _logger.LogInformation($"💓 HEARTBEAT para IP: {request.Ip}");

        try
        {
            // 1. Buscar ou criar Alphadigi
            var getOrCreateQuery = new GetOrCreateAlphadigiQuery { Ip = request.Ip };
            var alphadigi = await _mediator.Send(getOrCreateQuery, cancellationToken);

            _logger.LogInformation($"📷 Câmera encontrada: {alphadigi.Ip}, Estado: {alphadigi.Estado}");

            // 2. Processar estágio do Alphadigi
            var stageCommand = new HandleAlphadigiStageCommand { Alphadigi = alphadigi };
            var stageResponse = await _mediator.Send(stageCommand, cancellationToken);

            _logger.LogInformation($"🔄 Resposta do estágio: {(stageResponse != null ? "Tem dados" : "Null (FINAL)")}");

            // 3. Se a resposta do estágio NÃO for null (DELETE, CREATE, SEND), retorná-la
            if (stageResponse != null)
            {
                _logger.LogInformation($"📤 Retornando resposta do estágio: {stageResponse.GetType().Name}");
                return stageResponse;
            }

            // 4. 🔥 SE stageResponse É NULL (estágio FINAL), gerar dados do display
            _logger.LogInformation($"🎯 Estágio FINAL: Gerando display com sincronização");

            // Buscar nome do condomínio
            var condominio = await _condominioRepository.GetFirstAsync();
            var nomeCondominio = condominio?.Nome ?? "CONDOMINIO";

            // Determinar mensagem
            string linha1 = alphadigi.Sentido ? "BEM VINDO" : "ATE LOGO";

            _logger.LogInformation($"🖥️  Display: {linha1} - {nomeCondominio}");

            // Gerar pacotes do display (com sincronização de horário!)
            var serialData = await _displayService.RecieveMessageHearthbeatAlphadigi(
                linha1,
                nomeCondominio,
                alphadigi);

            // Log detalhado
            if (serialData != null)
            {
                _logger.LogInformation($"📦 {serialData.Count} pacotes gerados para display");
                for (int i = 0; i < serialData.Count; i++)
                {
                    var data = serialData[i];
                    _logger.LogInformation($"   #{i + 1}: {data.dataLen} bytes, Canal: {data.serialChannel}");

                    // Mostrar parte do primeiro pacote (sincronização)
                    if (i == serialData.Count - 1 && !string.IsNullOrEmpty(data.data))
                    {
                        try
                        {
                            var bytes = Convert.FromBase64String(data.data);
                            var hex = BitConverter.ToString(bytes).Replace("-", "");
                            _logger.LogInformation($"   🔧 Último pacote (sync?): {hex.Substring(0, Math.Min(30, hex.Length))}...");
                        }
                        catch { }
                    }
                }
            }

            // 5. Retornar no formato correto para AlphaDigi
            return new
            {
                Response_Heartbeat = new
                {
                    info = "no",
                    serialData = serialData ?? new List<SerialData>()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro em ProcessHeartbeat");
            throw;
        }
    }
}