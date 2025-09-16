using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Application.Queries.Unidade;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.DTOs.Veiculos;
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

            // 2. Buscar unidade pelo nome/código, sem conversão para int
            var unidade = await _mediator.Send(new GetUnidadeByNomeQuery(veiculoExistente.Unidade), cancellationToken);
            if (unidade == null)
            {
                _logger.LogWarning("Unidade {UnidadeNome} não encontrada para veículo {Placa}", veiculoExistente.Unidade, veiculo.Placa);
                return (false, "ACESSO NEGADO - UNIDADE INVÁLIDA");
            }

            string unidadeNome = unidade.Nome;
            _logger.LogInformation("Unidade encontrada: {Nome}", unidade.Nome);

            // 3. Verificar antipassback
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

            // 4. Determinar acesso baseado na direção da câmera
            if (alphadigi.Sentido) // Entrada
            {
                if (veiculoExistente.VeiculoDentro > 1)
                {
                    _logger.LogInformation($"Veículo {veiculo.Placa} já está dentro. Acesso NEGADO.");
                    return (false, "ACESSO NEGADO - JÁ ESTÁ DENTRO");
                }

                acesso = "LIBERADO - ENTRADA";
                shouldReturn = true;

                await _mediator.Send(new UpdateVagaVeiculoCommand
                {
                    Id = veiculoExistente.Id,
                    Dentro = true
                }, cancellationToken);
            }
            else // Saída
            {
                if (veiculoExistente.VeiculoDentro <= 0)
                {
                    _logger.LogInformation($"Veículo {veiculo.Placa} não está dentro. Acesso NEGADO.");
                    return (false, "ACESSO NEGADO - NÃO ESTÁ DENTRO");
                }

                acesso = "LIBERADO - SAÍDA";
                shouldReturn = true;

                await _mediator.Send(new UpdateVagaVeiculoCommand
                {
                    Id = veiculoExistente.Id,
                    Dentro = false
                }, cancellationToken);
            }

            // 5. Atualizar último acesso
            await _mediator.Send(new UpdateLastAccessCommand
            {
                IdVeiculo = veiculoExistente.Id,
                IpCamera = alphadigi.Ip,
                DataHoraAcesso = timestamp
            }, cancellationToken);

            // 6. Preparar dados para monitor
            var dadosMonitor = new DadosVeiculoMonitorDTO
            {
                Placa = veiculoExistente.Placa,
                Unidade = unidadeNome,
                Ip = alphadigi.Ip,
                Acesso = acesso,
                HoraAcesso = timestamp
            };

            var monitorCommand = new SendMonitorAcessoLinearCommand
            {
                DadosVeiculo = dadosMonitor,
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

