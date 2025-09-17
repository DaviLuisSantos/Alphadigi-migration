using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.PlacaLida;

public class UpdatePlacaLidaAcessoCommandHandler : IRequestHandler<UpdatePlacaLidaAcessoCommand>
{
    private readonly IPlacaLidaRepository _repository;
    private readonly ILogger<UpdatePlacaLidaAcessoCommandHandler> _logger;

    public UpdatePlacaLidaAcessoCommandHandler(
        IPlacaLidaRepository repository,
        ILogger<UpdatePlacaLidaAcessoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(UpdatePlacaLidaAcessoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando acesso da placa lida ID: {Id}", request.PlacaLidaId);

        var placaLida = await _repository.GetByIdAsync(request.PlacaLidaId);
        if (placaLida == null)
        {
            _logger.LogWarning("Placa lida não encontrada: {Id}", request.PlacaLidaId);
            return;
        }
        placaLida.AtualizarAcesso(request.Liberado, request.Acesso);



        await _repository.UpdatePlacaLidaAsync(placaLida);
        _logger.LogInformation("Placa lida atualizada: {Id} - Acesso: {Acesso}", request.PlacaLidaId, request.Acesso);
    }
}