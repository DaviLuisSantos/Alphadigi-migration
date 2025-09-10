using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Veiculo;

public class GetVeiculoByPlateQueryHandler : IRequestHandler<GetVeiculoByPlateQuery, Domain.EntitiesNew.Veiculo>
{
    private readonly IVeiculoRepository _repository;
    private readonly ILogger<GetVeiculoByPlateQueryHandler> _logger;

    public GetVeiculoByPlateQueryHandler(
        IVeiculoRepository repository,
        ILogger<GetVeiculoByPlateQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Veiculo> Handle(
        GetVeiculoByPlateQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando veículo pela placa: {Plate}", request.Plate);

        var veiculo = await _repository.GetByPlateAsync(request.Plate, request.MinMatchingCharacters);

        if (veiculo == null)
        {
            _logger.LogInformation("Veículo não encontrado para a placa: {Plate}", request.Plate);
            return null;
        }

        _logger.LogInformation("Veículo encontrado: {Plate} - ID: {Id}", veiculo.Placa, veiculo.Id);
        return veiculo;
    }
}