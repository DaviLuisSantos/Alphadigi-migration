using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class NaoControlaVagaAccessHandler : IRequestHandler<HandleAccessCommand, 
                                                           (bool ShouldReturn, string Acesso)>
{
    private readonly IMediator _mediator;
    private readonly ILogger<NaoControlaVagaAccessHandler> _logger;

    public NaoControlaVagaAccessHandler(
        IMediator mediator,
        ILogger<NaoControlaVagaAccessHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(HandleAccessCommand request, 
                                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Acesso sem controle de vaga para veículo: {request.Veiculo?.Placa}");

        try
        {
            string acesso = "CADASTRADO";
            bool abre = true;

            if (request.Veiculo != null)
            {
                if (!request.Alphadigi.Sentido) // Saída
                {
                    await _mediator.Send(new UpdateVagaVeiculoCommand
                    {
                        Id = request.Veiculo.Id,
                        Dentro = false
                    }, cancellationToken);

                    _logger.LogInformation($"Registrando saída para veículo {request.Veiculo.Placa}.");
                }
                else // Entrada
                {
                    await _mediator.Send(new UpdateVagaVeiculoCommand
                    {
                        Id = request.Veiculo.Id,
                        Dentro = true
                    }, cancellationToken);

                    _logger.LogInformation($"Registrando entrada para veículo {request.Veiculo.Placa}.");
                }
            }
            else
            {
                _logger.LogWarning($"Acesso negado: Veículo não encontrado");
                acesso = "NÃO CADASTRADO";
                abre = false;
            }

            return (abre, acesso);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro em NaoControlaVagaAccessHandler");
            throw;
        }
    }
}
