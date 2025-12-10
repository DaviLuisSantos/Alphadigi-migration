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
        bool enviado = false;
        bool marcadoComoEnviado = false;

        switch (stage)
        {
            case "DELETE":
                if (!alphadigi.Enviado)
                {
                    response = HandleDelete(alphadigi);
                    marcadoComoEnviado = true;
                }
                else
                {
                    newStage = "CREATE";
                    enviado = false;
                }
                break;
            case "CREATE":
                if (!alphadigi.Enviado)
                {
                    var createQuery = new HandleCreateQuery { Alphadigi = alphadigi };
                    response = await _mediator.Send(createQuery, cancellationToken);
                    newStage = "SEND";
                }
                else
                {
                    newStage = "SEND";
                    enviado = false;
                }
                break;
            case "SEND":
                _logger.LogInformation("📤 Executando SEND - Enviado: {Enviado}, UltimoId: {UltimoId}",
                    alphadigi.Enviado, alphadigi.UltimoId);

                var sendQuery = new HandleCreateQuery { Alphadigi = alphadigi };
                response = await _mediator.Send(sendQuery, cancellationToken);

                if (response == null)
                {
                    _logger.LogInformation("✅ Nenhum veículo encontrado após ID {UltimoId} - Indo para FINAL",
                        alphadigi.UltimoId);

                    // IR PARA FINAL
                    newStage = "FINAL";
                    alphadigi.ReiniciarEnvio();
                    alphadigi.AtualizarUltimoId(null); // Resetar para próximo ciclo
                }
                else
                {
                    _logger.LogInformation("📊 {Count} veículos encontrados - Mantendo SEND",
                        ((Domain.DTOs.Alphadigi.AddWhiteListDTO)response).AddWhiteList.Add_data.Count);

                    // Se era o primeiro envio do SEND
                    if (!alphadigi.Enviado)
                    {
                        alphadigi.MarcarComoEnviado();
                        // Atualizar UltimoId com o máximo enviado
                        // Isso depende da estrutura do response
                    }

                    // PERMANECER NO SEND para verificar mais dados
                    newStage = "SEND";
                }
                break;
            case "FINAL":
                var normalQuery = new HandleNormalQuery { Alphadigi = alphadigi };
                response = await _mediator.Send(normalQuery, cancellationToken);
                newStage = "FINAL";
                break;
            default:
                var defaultQuery = new HandleNormalQuery { Alphadigi = alphadigi };
                response = await _mediator.Send(defaultQuery, cancellationToken);
                newStage = "DELETE";
                break;
        }

        if (newStage != null)
        {
            alphadigi.AtualizarEstado(newStage);
        }

        alphadigi.MarcarComoEnviado();

        // Atualizar Alphadigi
        var updateCommand = new UpdateAlphadigiEntityCommand { Alphadigi = alphadigi };
        await _mediator.Send(updateCommand, cancellationToken);

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