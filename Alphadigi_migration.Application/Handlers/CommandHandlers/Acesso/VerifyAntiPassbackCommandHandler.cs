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

            _logger.LogInformation("Iniciando verificação antipassback para: {Placa}", veiculo.Placa);

            // Se veículo não tem ID, não tem antipassback
            if (veiculo.Id == Guid.Empty)
            {
                _logger.LogInformation("Veículo sem ID válido: {Placa} - antipassback ignorado", veiculo.Placa);
                return true; 
            }

            // Buscar câmera do último acesso
            if (string.IsNullOrEmpty(veiculo.IpCamUltAcesso))
            {
                _logger.LogInformation("Veículo sem IP da última câmera: {Placa} - antipassback ignorado", veiculo.Placa);
                return true;
            }

            var camUltQuery = new GetAlphadigiByIpQuery { Ip = veiculo.IpCamUltAcesso };
            var camUlt = await _mediator.Send(camUltQuery, cancellationToken);

            if (camUlt == null)
            {
                _logger.LogWarning("Câmera do último acesso não encontrada: {Ip} - antipassback ignorado", veiculo.IpCamUltAcesso);
                return true; 
            }

            // Verificar se está na mesma área
            if (camUlt?.AreaId != alphadigi.AreaId)
            {
                _logger.LogInformation(
                   "Câmeras em áreas diferentes - Última: {AreaIdUltima}, Atual: {AreaIdAtual} - antipassback ignorado",
                   camUlt.AreaId, alphadigi.AreaId);
                return true;
            }

            // Determinar tempo de antipassback
            var tempoAntipassback = alphadigi.Area?.TempoAntipassback ?? TimeSpan.FromSeconds(10);
            var timeLimit = timestamp - tempoAntipassback;

            // Verificar acessos recentes
            var acessoRecente = await _repository.VerifyAntiPassbackAsync(veiculo.Placa, timeLimit);

            var violouAntipassback = acessoRecente != null;

            _logger.LogInformation(
                "Resultado verificação antipassback para {Placa}: {Resultado}",
                veiculo.Placa, violouAntipassback ? "VIOLADO" : "LIBERADO");

            // Retorna true se NÃO violou antipassback (liberado)
            return !violouAntipassback;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao verificar antipassback para veículo {request.Veiculo.Placa}");
            return false; 
        }
    }
}


