using Alphadigi.Domain.Common;
using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class VerifyAntiPassbackCommandHandler : IRequestHandler<VerifyAntiPassbackCommand, bool>
{
    private readonly IAcessoRepository _repository;
    private readonly IMediator _mediator;
    private readonly ILogger<VerifyAntiPassbackCommandHandler> _logger;

    public VerifyAntiPassbackCommandHandler(IAcessoRepository acessoRepository, 
                                            IMediator mediator, 
                                            ILogger<VerifyAntiPassbackCommandHandler> logger)
    {
        _repository = acessoRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(VerifyAntiPassbackCommand request, 
                                  CancellationToken cancellationToken)
    {
        try
        {
            var veiculo = request.Veiculo;
            var alphadigi = request.Alphadigi;
            var timestamp = request.Timestamp;

            // Se veículo não tem ID, não tem antipassback
            if (veiculo.Id == null)
            {
                return false;
            }

            // Buscar câmera do último acesso
            if (string.IsNullOrEmpty(veiculo.IpCamUltAcesso))
            {
                return false;
            }

            var camUltQuery = new GetAlphadigiByIpQuery { Ip = veiculo.IpCamUltAcesso };
            var camUlt = await _mediator.Send(camUltQuery, cancellationToken);

            // Verificar se está na mesma área
            if (camUlt?.AreaId != alphadigi.AreaId)
            {
                return false;
            }

            // Determinar tempo de antipassback
            var tempoAntipassback = alphadigi.Area?.TempoAntipassbackTimeSpan ?? TimeSpan.FromSeconds(10);
            var timeLimit = timestamp - tempoAntipassback;

            // Verificar acessos recentes
            var recentAccesses = await _repository.VerifyAntiPassbackAsync(veiculo.Placa, timeLimit);

            _logger.LogInformation($"Verificação antipassback para {veiculo.Placa}: {(recentAccesses ? "DETECTADO" : "LIBERADO")}");
            return recentAccesses != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao verificar antipassback para veículo {request.Veiculo.Placa}");
            return false; 
        }
    }
}


