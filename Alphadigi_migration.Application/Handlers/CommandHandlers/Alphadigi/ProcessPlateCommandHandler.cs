using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.EntitiesNew;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class ProcessPlateCommandHandler : IRequestHandler<ProcessPlateCommand, object>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessPlateCommandHandler> _logger;

    public ProcessPlateCommandHandler(IMediator mediator, 
                                      ILogger<ProcessPlateCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<object> Handle(ProcessPlateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Plate value: '{Plate}'", request.Plate);
            _logger.LogInformation("Plate is null: {IsNull}", request.Plate == null);
            DateTime timeStamp = DateTime.Now;
            _logger.LogInformation("Placa recebida: {Plate} Camera: {Ip}", request.Plate, request.Ip);

            // Buscar ou criar câmera
            var camera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = request.Ip });

            if (camera == null)
            {
                _logger.LogError("Câmera não encontrada para o IP {Ip}.", request.Ip);
                throw new Exception("Camera não encontrada");
            }

            // Atualizar configuração do display se necessário
            if (camera.LinhasDisplay != 0 && request.Modelo == "TOTEM")
            {
                camera.AtualizarUltimoId(0);
                await _mediator.Send(new UpdateAlphadigiEntityCommand { Alphadigi = camera });
            }

            // Registrar placa lida
            var logCommand = new CreatePlacaLidaCommand
            {
                AlphadigiId = camera.Id,
                Placa = request.Plate,
                DataHora = timeStamp,
                AreaId = camera.AreaId,
                PlacaImg = request.PlateImage,
                CarroImg = request.CarImage
            };
            var log = await _mediator.Send(logCommand);

            // Verificar veículo
            var veiculoQuery = new GetVeiculoByPlateQuery { Plate = request.Plate, 
                                                            MinMatchingCharacters = 7 };


            var veiculoExistente = await _mediator.Send(veiculoQuery, cancellationToken);

            bool veiculoCadastrado = veiculoExistente != null;
            if (!veiculoCadastrado)
            {
                veiculoExistente = new Domain.EntitiesNew.Veiculo(
                    placa: request.Plate,
                    unidade: "TEMPORARIA",
                    marca: "NAO_CADASTRADO",
                    modelo: "NAO_CADASTRADO",
                    cor: "NAO_CADASTRADO"
                );
            }

            // Processar acesso do veículo
            var accessCommand = new SendVeiculoAccessProcessorCommand
            {
                Veiculo = veiculoExistente,
                Alphadigi = camera,
                Timestamp = timeStamp,
                Log = log,
                Imagem = request.CarImage
            };
            var accessResult = await _mediator.Send(accessCommand);

            _logger.LogInformation("Acesso do veículo com a placa {Plate} com resultado {ShouldReturn}",
                request.Plate, accessResult.ShouldReturn);

            // Atualizar placa lida com resultado do acesso
            await _mediator.Send(new UpdatePlacaLidaAcessoCommand
            {
                PlacaLidaId = log.Id,
                Liberado = accessResult.ShouldReturn,
                Acesso = accessResult.Acesso
            });

            // Criar pacote para display
            var displayQuery = new SendCreatePackageDisplayQuery
            {
                Veiculo = veiculoExistente,
                Acesso = accessResult.Acesso,
                Alphadigi = camera
            };
            var messageDisplay = await _mediator.Send(displayQuery);

            // CORREÇÃO: Remover o loop infinito e retornar objeto apropriado
            if (log.Processado)
            {
                var returnQuery = new HandleReturnQuery
                {
                    Placa = veiculoExistente.Placa,
                    Acesso = accessResult.Acesso,
                    Liberado = accessResult.ShouldReturn,
                    MessageData = messageDisplay
                };
                return await _mediator.Send(returnQuery);
            }

            // Retornar resultado do processamento
            return new
            {
                Placa = request.Plate,
                Liberado = accessResult.ShouldReturn,
                Acesso = accessResult.Acesso,
                MensagemDisplay = messageDisplay
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro em ProcessPlate.");
            throw;
        }
    }
}

