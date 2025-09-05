using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;

public class ProcessVeiculoAccessCommandHandler : IRequestHandler<ProcessVeiculoAccessCommand, 
                                                                 (bool ShouldReturn, string Acesso)>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessVeiculoAccessCommandHandler> _logger;

    public ProcessVeiculoAccessCommandHandler(IMediator mediator, 
                                              ILogger<ProcessVeiculoAccessCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(ProcessVeiculoAccessCommand request, 
                                                          CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Iniciando ProcessVeiculoAccessAsync para veículo: {request.Veiculo.Placa}");

        string acesso = "";
        bool shouldReturn = false;

        try
        {
            var veiculo = request.Veiculo;
            var alphadigi = request.Alphadigi;
            var timestamp = request.Timestamp;

            // 1. Verificar se veículo existe no banco
            var veiculoQuery = new GetVeiculoByPlateQuery { Plate = veiculo.Placa, MinMatchingCharacters = 7 };
            var veiculoExistente = await _mediator.Send(veiculoQuery, cancellationToken);

            if (veiculoExistente == null)
            {
                _logger.LogInformation($"Veículo {veiculo.Placa} não encontrado no banco. Acesso NEGADO.");
                return (false, "ACESSO NEGADO - VEÍCULO NÃO CADASTRADO");
            }

            // 2. Verificar unidade
            if (!string.IsNullOrEmpty(veiculoExistente.Unidade))
            {
                var unidadeQuery = new GetUnidadeByIdQuery { UnidadeId = veiculoExistente.Unidade };
                var unidade = await _mediator.Send(unidadeQuery, cancellationToken);

                if (unidade == null)
                {
                    _logger.LogWarning($"Unidade {veiculoExistente.Unidade} não encontrada para veículo {veiculo.Placa}");
                    return (false, "ACESSO NEGADO - UNIDADE INVÁLIDA");
                }

                // 3. Verificar se unidade está ativa
                if (!unidade.EstaAtiva())
                {
                    _logger.LogInformation($"Unidade {veiculoExistente.Unidade} inativa. Acesso NEGADO.");
                    return (false, "ACESSO NEGADO - UNIDADE INATIVA");
                }
            }

            // 4. Verificar antipassback
            var antipassbackCommand = new VerifyAntiPassbackCommand
            {
                Veiculo = veiculoExistente,
                Alphadigi = alphadigi,
                Timestamp = timestamp
            };
            var antipassbackPermitido = await _mediator.Send(antipassbackCommand, cancellationToken);

            if (!antipassbackPermitido)
            {
                _logger.LogInformation($"Antipassback violado para veículo {veiculo.Placa}. Acesso NEGADO.");
                return (false, "ACESSO NEGADO - ANTIPASSBACK");
            }

            // 5. Determinar acesso baseado na direção da câmera
            if (alphadigi.Sentido) // Entrada
            {
                if (veiculoExistente.VeiculoDentro)
                {
                    _logger.LogInformation($"Veículo {veiculo.Placa} já está dentro. Acesso NEGADO.");
                    return (false, "ACESSO NEGADO - JÁ ESTÁ DENTRO");
                }

                acesso = "LIBERADO - ENTRADA";
                shouldReturn = true;

                // Marcar veículo como dentro
                await _mediator.Send(new UpdateVagaVeiculoCommand
                {
                    Id = veiculoExistente.Id,
                    Dentro = true
                }, cancellationToken);
            }
            else // Saída
            {
                if (!veiculoExistente.VeiculoDentro)
                {
                    _logger.LogInformation($"Veículo {veiculo.Placa} não está dentro. Acesso NEGADO.");
                    return (false, "ACESSO NEGADO - NÃO ESTÁ DENTRO");
                }

                acesso = "LIBERADO - SAÍDA";
                shouldReturn = true;

                // Marcar veículo como fora
                await _mediator.Send(new UpdateVagaVeiculoCommand
                {
                    Id = veiculoExistente.Id,
                    Dentro = false
                }, cancellationToken);
            }

            // 6. Atualizar último acesso
            await _mediator.Send(new UpdateLastAccessCommand
            {
                IdVeiculo = veiculoExistente.Id,
                IpCamera = alphadigi.Ip,
                DataHoraAcesso = timestamp
            }, cancellationToken);

            // 7. Enviar para monitor linear
            var monitorCommand = new SendMonitorAcessoLinearCommand
            {
                Veiculo = veiculoExistente,
                IpCamera = alphadigi.Ip,
                Acesso = acesso,
                Timestamp = timestamp
            };
            await _mediator.Send(monitorCommand, cancellationToken);

            _logger.LogInformation($"Acesso {acesso} para veículo {veiculo.Placa}");
            return (shouldReturn, acesso);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao processar acesso do veículo.");
            return (false, "ERRO NO PROCESSAMENTO");
        }
    }
}
    
