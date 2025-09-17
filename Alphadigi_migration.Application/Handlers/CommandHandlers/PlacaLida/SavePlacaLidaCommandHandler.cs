using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.PlacaLida;

public class SavePlacaLidaCommandHandler : IRequestHandler<SavePlacaLidaCommand, bool>
{
    private readonly IPlacaLidaRepository _repository;
    private readonly ILogger<SavePlacaLidaCommandHandler> _logger;

    public SavePlacaLidaCommandHandler(
        IPlacaLidaRepository repository,
        ILogger<SavePlacaLidaCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(SavePlacaLidaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salvando placa lida: {Placa}", request.Placa);

        var placaLida = new Domain.EntitiesNew.PlacaLida(
            request.Placa,
            request.AlphadigiId,
            request.AreaId,
            request.DataHora,
            request.CarroImg,
            request.PlacaImg,
            request.Real,
            request.Cadastrado);

        return await _repository.SavePlacaLidaAsync(placaLida);
    }
}