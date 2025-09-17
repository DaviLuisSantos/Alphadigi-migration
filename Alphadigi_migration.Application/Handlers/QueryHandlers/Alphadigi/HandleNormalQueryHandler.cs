using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class HandleNormalQueryHandler : IRequestHandler<HandleNormalQuery, Domain.DTOs.Alphadigi.ResponseHeathbeatDTO>
{
    private readonly IMediator _mediator;
    private readonly ILogger<HandleNormalQueryHandler> _logger;

    public HandleNormalQueryHandler(
        IMediator mediator,
        ILogger<HandleNormalQueryHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Domain.DTOs.Alphadigi.ResponseHeathbeatDTO> Handle(HandleNormalQuery request, CancellationToken cancellationToken)
    {
        var alphadigi = request.Alphadigi;

        // Buscar condomínio usando query
        var condominioQuery = new GetCondominioQuery();
        var condominio = await _mediator.Send(condominioQuery, cancellationToken);

        var nome = condominio.Nome;

        // Buscar dados do display usando query
        var linha1 = alphadigi.Sentido ? "BEM VINDO" : "ATE LOGO";
        var displayQuery = new SendDisplayQuery
        {
            Nome = nome,
            Alphadigi = alphadigi
        };
        var messageData = await _mediator.Send(displayQuery, cancellationToken);

        var retorno = new Domain.DTOs.Alphadigi.ResponseHeathbeatDTO
        {
            Response_Heartbeat = new Domain.DTOs.Alphadigi.ResponseAlarmInfoPlate
            {
                info = "no",
                serialData = messageData
            }
        };

        return retorno;
    }
}
