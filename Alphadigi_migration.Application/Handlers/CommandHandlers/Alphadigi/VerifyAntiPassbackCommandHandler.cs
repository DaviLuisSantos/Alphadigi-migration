using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class VerifyAntiPassbackCommandHandler : IRequestHandler<VerifyAntiPassbackCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<VerifyAntiPassbackCommandHandler> _logger;

    public VerifyAntiPassbackCommandHandler(IMediator mediator, 
                                              ILogger<VerifyAntiPassbackCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(VerifyAntiPassbackCommand request, 
                                   CancellationToken cancellationToken)
    {
        var veiculo = request.Veiculo;
        var alphadigi = request.Alphadigi;
        var timestamp = request.Timestamp;

        bool mesmaCamera = veiculo.IpCamUltAcesso == alphadigi.Ip;
        var area = alphadigi.Area;

        if (mesmaCamera)
        {
            // Lógica para mesma câmera
            return await ProcessMesmaCamera(area, veiculo, timestamp);
        }
        else
        {
            // Lógica para câmeras diferentes
            return await ProcessCameraDiferente(veiculo, alphadigi, timestamp);
        }
    }

    private async Task<bool> ProcessMesmaCamera(Domain.EntitiesNew.Area area, Domain.EntitiesNew.Veiculo veiculo, DateTime timestamp)
    {
        if (veiculo.DataHoraUltAcesso.HasValue)
        {
            var tempoAntipassback = area.TempoAntipassbackTimeSpan.Value;
            var dentroDoPassback = timestamp - veiculo.DataHoraUltAcesso < tempoAntipassback;

            if (dentroDoPassback)
            {
                return false;
            }
        }
        return true;
    }

    private async Task<bool> ProcessCameraDiferente(Domain.EntitiesNew.Veiculo veiculo, Domain.EntitiesNew.Alphadigi alphadigi, DateTime timestamp)
    {
        if (!string.IsNullOrEmpty(veiculo.IpCamUltAcesso))
        {
            var ultimaCamera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = veiculo.IpCamUltAcesso });
            var mesmaArea = ultimaCamera.AreaId == alphadigi.AreaId;

            if (mesmaArea && veiculo.DataHoraUltAcesso.HasValue)
            {
                var area = alphadigi.Area;
                var tempoAntipassback = area.TempoAntipassbackTimeSpan.Value;
                var dentroDoPassback = timestamp - veiculo.DataHoraUltAcesso < tempoAntipassback;

                if (dentroDoPassback)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
