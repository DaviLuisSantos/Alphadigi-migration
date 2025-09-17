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

            // 1. Buscar câmera. Se não existir, abortar.
            var camera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = request.Ip });
            if (camera == null)
            {
                _logger.LogError("Câmera não encontrada para o IP {Ip}. Abortando.", request.Ip);
                throw new Exception("Camera não encontrada");
            }

            // 2. Registrar a leitura da placa.
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

            // 3. Buscar veículo. Se não encontrar, criar um temporário.
           
            
            var veiculo = await _mediator.Send(new GetVeiculoByPlateQuery { Plate = request.Plate, MinMatchingCharacters = 7 });
            if (veiculo == null)
            {
                veiculo = new Domain.EntitiesNew.Veiculo(request.Plate, "SEM UNIDADE", "NAO CADASTRADO", "NAO CADASTRADO", "NAO CADASTRADO");
                _logger.LogInformation($"Veículo não cadastrado. Criado temporariamente para placa: {veiculo.Placa}");
            }
            //if (veiculo == null)
            //{
            //    _logger.LogInformation($"Veículo não cadastrado encontrado. Criado temporariamente para placa: {request.Plate}");

            
            //   veiculo = Domain.EntitiesNew.Veiculo.CreateUnregistered(request.Plate);
            //}

            // 4. Processar o acesso e obter o resultado.
            var accessResult = await _mediator.Send(new ProcessVeiculoAccessCommand
            {
                Veiculo = veiculo,
                Alphadigi = camera,
                Timestamp = timeStamp,
                //Log = log,
                //Imagem = request.CarImage
            });

            // 5. Se o acesso for liberado, salve-o.
            if (accessResult.ShouldReturn)
            {
                _logger.LogInformation(" Acesso LIBERADO para veículo {Plate}", request.Plate);
                await _mediator.Send(new SaveVeiculoAcessoCommand
                {
                    Veiculo = veiculo,
                    Alphadigi = camera,
                    Timestamp = timeStamp,
                    Imagem = request.CarImage
                });
                _logger.LogInformation("Acesso salvo para veículo {Plate}", request.Plate);
            }
            else
            {
                _logger.LogInformation(" Acesso NEGADO para veículo {Plate}. Motivo: {Acesso}", request.Plate, accessResult.Acesso);
            }

            // 6. Atualize o log da placa lida e envie para o display e monitor,
            //    independentemente do resultado do acesso.
            await _mediator.Send(new UpdatePlacaLidaAcessoCommand
            {
                PlacaLidaId = log.Id,
                Liberado = accessResult.ShouldReturn,
                Acesso = accessResult.Acesso
            });

            await _mediator.Send(new SendToDisplayCommand
            {
                Placa = request.Plate,
                Acesso = accessResult.Acesso,
                Liberado = accessResult.ShouldReturn,
                Alphadigi = camera,
                Veiculo = veiculo
            });

            var dadosVeiculoDTO = new DadosVeiculoMonitorDTO
            {
                Placa = veiculo.Placa.Numero,
                Unidade = veiculo.Unidade, 
                Ip = request.Ip,
                Acesso = accessResult.Acesso,
                HoraAcesso = timeStamp,
                Modelo = veiculo.Modelo,
                Marca = veiculo.Marca,
                Cor = veiculo.Cor

            };


            string dadosVeiculoStr = $"{dadosVeiculoDTO.Modelo ?? "INDEFINIDO"} - {dadosVeiculoDTO.Marca ?? "INDEFINIDO"} - {dadosVeiculoDTO.Cor ?? "INDEFINIDO"}";

            await _mediator.Send(new SendMonitorAcessoLinearCommand
            {
                DadosVeiculo = dadosVeiculoDTO,
                 IpCamera = request.Ip, 
                Acesso = accessResult.Acesso, 
                Timestamp = timeStamp,
                DadosVeiculoStr = dadosVeiculoStr
            });
          

            _logger.LogInformation(" DADOS ENVIADOS PARA MONITOR ACESSO LINEAR.");

            return new
            {
                Placa = request.Plate,
                Liberado = accessResult.ShouldReturn,
                Acesso = accessResult.Acesso,
                
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro em ProcessPlate. Abortando.");
            throw;
        }
    }
}