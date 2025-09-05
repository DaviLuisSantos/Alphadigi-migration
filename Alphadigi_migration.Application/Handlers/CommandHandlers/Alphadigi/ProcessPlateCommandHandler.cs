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

    public async Task<object> Handle(ProcessPlateCommand request, 
                                     CancellationToken cancellationToken)
    {
        try
        {
            DateTime timeStamp = DateTime.Now;
            _logger.LogInformation($"Placa recebida: {request.Plate} Camera: {request.Ip}");

            // Buscar ou criar câmera
            var camera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = request.Ip });

            if (camera == null)
            {
                _logger.LogError($"Câmera não encontrada para o IP {request.Ip}.");
                throw new Exception("Camera não encontrada");
            }

            // Atualizar configuração do display se necessário
            if (camera.LinhasDisplay != 0 && request.Modelo == "TOTEM")
            {
                camera.AtualizarUltimoId(Guid.Empty);
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
            var veiculoQuery = new GetVeiculoByPlateQuery { Plate = request.Plate };
            var veiculo = await _mediator.Send(veiculoQuery);

            bool veiculoCadastrado = veiculo != null;
            if (!veiculoCadastrado)
            {
                veiculo = new Domain.EntitiesNew.Veiculo(
                  placa: request.Plate,
                  unidade: "TEMPORARIA", 
                   marca: "NAO_CADASTRADO", 
                    modelo: "NAO_CADASTRADO", 
                  cor: "NAO_CADASTRADO" 
    );
            }

            // Atualizar estado da câmera se necessário
            if (veiculo != null && !request.IsCad && veiculoCadastrado)
            {
                camera.AtualizarUltimoId(Guid.Empty);
                await _mediator.Send(new UpdateAlphadigiEntityCommand { Alphadigi = camera });
            }

            // Atualizar placa lida com informação de cadastro
            await _mediator.Send(new UpdatePlacaLidaCadastroCommand
            {
                PlacaLidaId = log.Id,
                Cadastrado = veiculoCadastrado
            });

            // Processar acesso do veículo
            var accessCommand = new SendVeiculoAccessProcessorCommand
            {
                Veiculo = veiculo,
                Alphadigi = camera,
                Timestamp = timeStamp,
                Log = log,
                Imagem = request.CarImage
            };
            var accessResult = await _mediator.Send(accessCommand);

            _logger.LogInformation($"Acesso do veículo com a placa {request.Plate} com resultado {accessResult.ShouldReturn}");

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
                Veiculo = veiculo,
                Acesso = accessResult.Acesso,
                Alphadigi = camera
            };
            var messageDisplay = await _mediator.Send(displayQuery);

            if (log.Processado)
            {
                var returnQuery = new HandleReturnQuery
                {
                    Placa = veiculo.Placa,
                    Acesso = accessResult.Acesso,
                    Liberado = accessResult.ShouldReturn,
                    MessageData = messageDisplay
                };
                return await _mediator.Send(returnQuery);
            }

            return await Handle(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro em ProcessPlate.");
            throw;
        }
    }
}

