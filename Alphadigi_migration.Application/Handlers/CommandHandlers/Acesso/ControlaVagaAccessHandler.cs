using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Application.Queries.Unidade;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class ControlaVagaAccessHandler : IRequestHandler<HandleAccessCommand, (bool ShouldReturn, string Acesso)>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ControlaVagaAccessHandler> _logger;

    public ControlaVagaAccessHandler(
        IMediator mediator,
        ILogger<ControlaVagaAccessHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(HandleAccessCommand request, 
                                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Gerenciando controle de vaga para veículo: {request.Veiculo?.Placa}");

        try
        {
            string acesso = "";
            bool abre = true;

            if (!request.Alphadigi.Sentido) // Saída
            {
                if (request.Veiculo != null)
                {
                    await _mediator.Send(new UpdateVagaVeiculoCommand
                    {
                        Id = request.Veiculo.Id,
                        Dentro = false
                    }, cancellationToken);

                    _logger.LogInformation($"Liberando vaga para veículo {request.Veiculo.Placa}.");
                }
                acesso = "SAÍDA LIBERADA";
            }
            else // Entrada
            {
                if (request.Veiculo != null && !string.IsNullOrEmpty(request.Veiculo.Unidade))
                {
                    var vagasQuery = new GetUnidadeVagasQuery { UnidadeId = request.Veiculo.Unidade };
                    var vagas = await _mediator.Send(vagasQuery, cancellationToken);

                    if (vagas != null && (vagas.NumVagas > vagas.VagasOcupadas || vagas.NumVagas > request.Veiculo.VeiculoDentro ))
                    {
                        await _mediator.Send(new UpdateVagaVeiculoCommand
                        {
                            Id = request.Veiculo.Id,
                            Dentro = true
                        }, cancellationToken);

                        _logger.LogInformation($"Concedendo acesso e ocupando vaga para veículo {request.Veiculo.Placa}.");
                        acesso = "ENTRADA LIBERADA";
                    }
                    else
                    {
                        acesso = "S/VG - SEM VAGA";
                        abre = false;
                    }
                }
                else
                {
                    _logger.LogWarning($"Não foi possível obter informações da unidade para veículo {request.Veiculo?.Placa}");
                    acesso = "S/VG - UNIDADE INVÁLIDA";
                    abre = false;
                }
            }

            return (abre, acesso);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro em ControlaVagaAccessHandler");
            throw;
        }
    }
}
