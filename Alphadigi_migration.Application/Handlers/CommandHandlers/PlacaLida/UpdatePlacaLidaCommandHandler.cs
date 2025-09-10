using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.PlacaLida;

public class UpdatePlacaLidaCommandHandler : IRequestHandler<UpdatePlacaLidaCommand, bool>
{
    private readonly IPlacaLidaRepository _repository;
    private readonly ILogger<UpdatePlacaLidaCommandHandler> _logger;

    public UpdatePlacaLidaCommandHandler(IPlacaLidaRepository repository, 
                                         ILogger<UpdatePlacaLidaCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdatePlacaLidaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando placa lida: {Id}", request.Id);

        var placaLida = await _repository.GetByIdAsync(request.Id);

        if (placaLida == null)
        {
            _logger.LogWarning("Placa lida não encontrada: {Id}", request.Id);
            return false;
        }

        if (request.Liberado.HasValue && request.Acesso != null)
        {
            placaLida.MarcarComoProcessado(request.Liberado.Value, request.Acesso);
        }

        if (request.Cadastrado.HasValue)
        {
            placaLida.AtualizarCadastro(request.Cadastrado.Value);
        }

        if (request.Processado.HasValue && !request.Processado.Value)
        {
            placaLida = new Domain.EntitiesNew.PlacaLida(
                placaLida.Placa.ToString(),
                placaLida.AlphadigiId,
                placaLida.AreaId,
                placaLida.DataHora,
                placaLida.CarroImg,
                placaLida.PlacaImg,
                placaLida.Real,
                placaLida.Cadastrado,
                placaLida.Processado,
                placaLida.Liberado,
                null);
        }

        if (request.CarroImg != null || request.PlacaImg != null)
        {
            placaLida.AtualizarImagens(request.CarroImg, request.PlacaImg);
        }

        if (request.Real.HasValue && !request.Real.Value)
        {
            placaLida.MarcarComoNaoReal();
        }

        return await _repository.UpdatePlacaLidaAsync(placaLida);
    }
}
