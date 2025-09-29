using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Commands.Display;
using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.DTOs.Veiculos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class ProcessPlateCommandHandler : IRequestHandler<ProcessPlateCommand, object>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessPlateCommandHandler> _logger;

    public ProcessPlateCommandHandler(IMediator mediator, ILogger<ProcessPlateCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<object> Handle(ProcessPlateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Início do processamento da placa: {Plate}, IP da câmera: {Ip}", request.Plate, request.Ip);

        try
        {
            DateTime timeStamp = DateTime.Now;

            // 1. Buscar câmera
            var camera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = request.Ip });
            if (camera == null)
            {
                _logger.LogError("Câmera não encontrada para o IP {Ip}. Abortando.", request.Ip);
                throw new Exception("Camera não encontrada");
            }

            // 2. Registrar a leitura da placa
            var log = await _mediator.Send(new CreatePlacaLidaCommand
            {
                AlphadigiId = camera.Id,
                Placa = request.Plate,
                DataHora = timeStamp,
                AreaId = camera.AreaId,
                PlacaImg = request.PlateImage,
                CarroImg = request.CarImage
            });
            _logger.LogInformation("Placa lida registrada com ID: {LogId}", log.Id);

            // 3. Buscar veículo
            var veiculo = await _mediator.Send(new GetVeiculoByPlateQuery { Plate = request.Plate, MinMatchingCharacters = 7 });
            if (veiculo == null)
            {
                veiculo = new Domain.EntitiesNew.Veiculo(request.Plate, "SEM UNIDADE", "NAO CADASTRADO", "NAO CADASTRADO", "NAO CADASTRADO");
                _logger.LogInformation($"Veículo não cadastrado. Criado temporariamente para placa: {veiculo.Placa}");
            }

            // 4. Processar acesso
            var accessResult = await _mediator.Send(new ProcessVeiculoAccessCommand
            {
                Veiculo = veiculo,
                Alphadigi = camera,
                Timestamp = timeStamp
            });

            // 5. Salvar acesso se liberado
            if (accessResult.ShouldReturn)
            {
                _logger.LogInformation("Acesso LIBERADO para veículo {Plate}", request.Plate);
                await _mediator.Send(new SaveVeiculoAcessoCommand
                {
                    Veiculo = veiculo,
                    Alphadigi = camera,
                    Timestamp = timeStamp,
                    Imagem = request.CarImage
                });
            }
            else
            {
                _logger.LogInformation("Acesso NEGADO para veículo {Plate}. Motivo: {Acesso}", request.Plate, accessResult.Acesso);
            }

            // 6. Atualizar log da placa lida
            await _mediator.Send(new UpdatePlacaLidaAcessoCommand
            {
                PlacaLidaId = log.Id,
                Liberado = accessResult.ShouldReturn,
                Acesso = accessResult.Acesso
            });

            // 7. Enviar para display
            await _mediator.Send(new SendToDisplayCommand
            {
                Placa = request.Plate,
                Acesso = accessResult.Acesso,
                Liberado = accessResult.ShouldReturn,
                Alphadigi = camera,
                Veiculo = veiculo
            });

            // 8. Enviar dados para monitor (uma única vez)
            var dadosVeiculoDTO = new DadosVeiculoMonitorDTO
            {
                Placa = veiculo.Placa?.Numero ?? request.Plate,
                Unidade = veiculo.Unidade ?? "NAO CADASTRADO",
                Ip = request.Ip,
                Acesso = accessResult.Acesso,
                HoraAcesso = timeStamp,
                Modelo = veiculo.Modelo ?? "INDEFINIDO",
                Marca = veiculo.Marca ?? "INDEFINIDO",
                Cor = veiculo.Cor ?? "INDEFINIDO"
            };

            string dadosVeiculoStr = $"{dadosVeiculoDTO.Modelo} - {dadosVeiculoDTO.Marca} - {dadosVeiculoDTO.Cor}";

            await _mediator.Send(new SendMonitorAcessoLinearCommand
            {
                DadosVeiculo = dadosVeiculoDTO,
                IpCamera = request.Ip,
                Acesso = accessResult.Acesso,
                Timestamp = timeStamp,
                DadosVeiculoStr = dadosVeiculoStr
            });

            _logger.LogInformation("Dados enviados para monitor de acesso linear com sucesso.");

            return new
            {
                Placa = request.Plate,
                Liberado = accessResult.ShouldReturn,
                Acesso = accessResult.Acesso
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro em ProcessPlate. Abortando.");
            throw;
        }
    }
}
