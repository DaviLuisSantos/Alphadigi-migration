using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class HandleAlphadigiStageCommandHandler : IRequestHandler<HandleAlphadigiStageCommand, object>
{
    private readonly IMediator _mediator;
    private readonly ILogger<HandleAlphadigiStageCommandHandler> _logger;

    public HandleAlphadigiStageCommandHandler(
        IMediator mediator,
        ILogger<HandleAlphadigiStageCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<object> Handle(HandleAlphadigiStageCommand request,
                                     CancellationToken cancellationToken)
    {
        var alphadigi = request.Alphadigi;
        string stage = alphadigi.Estado ?? "DELETE";
        string newStage = null;
        object response = null;

        _logger.LogInformation($"🔄 Processando estágio: {stage} para câmera {alphadigi.Ip}");

        switch (stage)
        {
            case "DELETE":
                if (!alphadigi.Enviado)
                {
                    response = HandleDelete(alphadigi);
                    _logger.LogInformation($"🗑️  Estágio DELETE: retornando limpeza de whitelist");
                }
                else
                {
                    newStage = "CREATE";
                    alphadigi.MarcarComoNaoEnviado(); // Reset para próximo estágio
                }
                break;

            case "CREATE":
                if (!alphadigi.Enviado)
                {
                    var createQuery = new HandleCreateQuery { Alphadigi = alphadigi };
                    response = await _mediator.Send(createQuery, cancellationToken);
                    newStage = "SEND";
                    _logger.LogInformation($"📝 Estágio CREATE: {(response != null ? "Whitelist gerada" : "Nada para enviar")}");
                }
                else
                {
                    newStage = "SEND";
                    alphadigi.MarcarComoNaoEnviado();
                }
                break;

            case "SEND":
                var sendQuery = new HandleCreateQuery { Alphadigi = alphadigi };
                response = await _mediator.Send(sendQuery, cancellationToken);

                if (response == null)
                {
                    newStage = "FINAL";
                    _logger.LogInformation($"✅ Estágio SEND: Todas as whitelists enviadas. Indo para FINAL");
                }
                else
                {
                    _logger.LogInformation($"📤 Estágio SEND: Enviando lote de whitelists");
                }
                break;

            case "FINAL":
                // 🔥 IMPORTANTE: No estágio FINAL, NÃO retornamos um objeto de resposta!
                // Deixamos o ProcessHeartbeatCommandHandler retornar os dados do display
                _logger.LogInformation($"🎯 Estágio FINAL: Mantendo estágio (display será gerenciado pelo heartbeat)");
                newStage = "FINAL";
                response = null; // 🔥 CRÍTICO: Null para o heartbeat assumir
                break;

            default:
                _logger.LogWarning($"⚠️  Estágio desconhecido: {stage}. Resetando para DELETE");
                response = HandleDelete(alphadigi);
                newStage = "DELETE";
                break;
        }

        if (newStage != null)
        {
            alphadigi.AtualizarEstado(newStage);
            _logger.LogInformation($"📊 Novo estágio: {newStage}");
        }

        alphadigi.MarcarComoEnviado();

        // Atualizar Alphadigi
        var updateCommand = new UpdateAlphadigiEntityCommand { Alphadigi = alphadigi };
        await _mediator.Send(updateCommand, cancellationToken);

        _logger.LogInformation($"🏁 Estágio {stage} processado. Response é null? {response == null}");
        return response;
    }

    private Domain.DTOs.Alphadigi.DeleteWhiteListAllDTO HandleDelete(Domain.EntitiesNew.Alphadigi alphadigi)
    {
        return new Domain.DTOs.Alphadigi.DeleteWhiteListAllDTO
        {
            DeleteWhiteListAll = 1
        };
    }
}