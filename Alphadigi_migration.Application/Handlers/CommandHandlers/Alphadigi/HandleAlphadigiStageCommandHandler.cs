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

        switch (stage)
        {
            case "DELETE":
                if (!alphadigi.Enviado)
                {
                    response = HandleDelete(alphadigi);
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
                var sendQuery = new HandleCreateQuery { Alphadigi = alphadigi };
                response = await _mediator.Send(sendQuery, cancellationToken);
                if (response == null)
                {
                    newStage = "FINAL";
                }
                if (enviado)
                {
                    newStage = response == null ? "FINAL" : "SEND";
                    enviado = false;
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