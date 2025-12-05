using Alphadigi_migration.Application.Queries.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class HandleReturnQueryHandler : IRequestHandler<HandleReturnQuery, Domain.DTOs.Alphadigi.ResponsePlateDTO>
{
    private readonly ILogger<HandleReturnQueryHandler> _logger;

    public HandleReturnQueryHandler(ILogger<HandleReturnQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<Domain.DTOs.Alphadigi.ResponsePlateDTO> Handle(HandleReturnQuery request, CancellationToken cancellationToken)
    {
        string info = request.Liberado ? "ok" : "no";

        var retorno = new Domain.DTOs.Alphadigi.ResponsePlateDTO
        {
            Response_AlarmInfoPlate = new Domain.DTOs.Alphadigi.Response_AlarmInfoPlate
            {
                info = info,
                serialData = request.MessageData
            }
        };

        return Task.FromResult(retorno);
    }
}