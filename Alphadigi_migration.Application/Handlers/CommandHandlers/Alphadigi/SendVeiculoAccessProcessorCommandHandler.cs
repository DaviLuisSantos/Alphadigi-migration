using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Commands.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;
public class SendVeiculoAccessProcessorCommandHandler : IRequestHandler<SendVeiculoAccessProcessorCommand, 
                                                                        (bool ShouldReturn, string Acesso)>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendVeiculoAccessProcessorCommandHandler> _logger;

    public SendVeiculoAccessProcessorCommandHandler(
        IMediator mediator,
        ILogger<SendVeiculoAccessProcessorCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(SendVeiculoAccessProcessorCommand request, 
                                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processando acesso do veículo {request.Veiculo.Placa} na câmera {request.Alphadigi.Ip}");

        try
        {
            // 1. Processar o acesso do veículo
            var accessCommand = new ProcessVeiculoAccessCommand
            {
                Veiculo = request.Veiculo,
                Alphadigi = request.Alphadigi,
                Timestamp = request.Timestamp
            };

            var (shouldReturn, acesso) = await _mediator.Send(accessCommand, cancellationToken);

            // 2. Atualizar último acesso no banco
            if (request.Veiculo.Id != Guid.Empty) // Verifica se é um veículo persistido
            {
                var updateAccessCommand = new UpdateLastAccessCommand
                {
                    IdVeiculo = request.Veiculo.Id,
                    IpCamera = request.Alphadigi.Ip,
                    DataHoraAcesso = request.Timestamp
                };

                await _mediator.Send(updateAccessCommand, cancellationToken);
            }

            // 3. Salvar acesso no histórico
            var saveAcessoCommand = new SaveVeiculoAcessoCommand
            {
                Alphadigi = request.Alphadigi,
                Veiculo = request.Veiculo,
                Timestamp = request.Timestamp,
                Imagem = request.Imagem,
                AcessoPermitido = shouldReturn,
                MotivoAcesso = acesso
            };

            await _mediator.Send(saveAcessoCommand, cancellationToken);

            _logger.LogInformation($"Acesso processado: {acesso} para veículo {request.Veiculo.Placa}");

            return (shouldReturn, acesso);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao processar acesso do veículo {request.Veiculo.Placa}");
            return (false, "ERRO NO PROCESSAMENTO");
        }
    }

}

