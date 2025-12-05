using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
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
        bool enviado = false; // 🔥 ADICIONADO: Variável enviado como no código antigo

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
                    enviado = false; // 🔥 RESET enviado como no código antigo
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
                    enviado = false; // 🔥 RESET enviado como no código antigo
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

                // 🔥 ADICIONADO: Lógica do código antigo para enviado
                if (enviado)
                {
                    newStage = response == null ? "FINAL" : "SEND";
                    enviado = false;
                }
                break;

            case "FINAL":

                _logger.LogInformation($"[{DateTime.Now:HH:mm:ss.fff}] 🎯 ESTÁGIO FINAL: Enviando mensagem padrão para display");

              

                // ⭐⭐ NÃO FAÇA RETURN AQUI! Configure a response e deixe o fluxo continuar
                response = new ResponseHeathbeatDTO
                {
                    Response_Heartbeat = new Response_AlarmInfoPlate
                    {
                        info = "no",
                        serialData = new List<SerialData>()
                    }
                };

                newStage = "FINAL"; // ⭐ Mantém no mesmo estágio
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

        // 🔥 CORREÇÃO: Usar MarcarComoEnviado() se tiver, senão:
        alphadigi.MarcarComoEnviado(); // No código antigo sempre marcava como enviado após processar estágio

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