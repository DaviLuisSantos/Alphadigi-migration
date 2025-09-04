using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class SaidaSempreAbreAccessHandler : IRequestHandler<HandleAccessCommand, (bool ShouldReturn, string Acesso)>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SaidaSempreAbreAccessHandler> _logger;

    public SaidaSempreAbreAccessHandler(
        IMediator mediator,
        ILogger<SaidaSempreAbreAccessHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(HandleAccessCommand request, 
                                                                CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processando saída que sempre abre");

        try
        {
            string acesso = "NÃO CADASTRADO";

            if (request.Veiculo != null && request.Veiculo.Id != null)
            {
                _logger.LogInformation($"Aprovando saída do veículo cadastrado com ID {request.Veiculo.Id}.");

                // Usar command em vez de service direto
                await _mediator.Send(new UpdateVagaVeiculoCommand
                {
                    Id = request.Veiculo.Id,
                    Dentro = false
                }, cancellationToken);

                acesso = "CADASTRADO";
            }
            else
            {
                _logger.LogInformation("Aprovando saída para veículo não cadastrado.");
            }

            return (true, acesso);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro em SaidaSempreAbreAccessHandler");
            throw;
        }
    }
}