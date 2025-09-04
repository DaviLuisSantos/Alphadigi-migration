using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Display;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class SendDisplayQueryHandler : IRequestHandler<SendDisplayQuery, List<Domain.DTOs.Alphadigi.SerialData>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendDisplayQueryHandler> _logger;

    public SendDisplayQueryHandler(
        IMediator mediator,
        ILogger<SendDisplayQueryHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<List<Domain.DTOs.Alphadigi.SerialData>> Handle(SendDisplayQuery request, CancellationToken cancellationToken)
    {
        // Usar query específica para display
        var displayQuery = new RecieveMessageAlphadigiQuery
        {
            Linha1 = request.Nome,
            Alphadigi = request.Alphadigi
        };

        return await _mediator.Send(displayQuery, cancellationToken);
    }
}