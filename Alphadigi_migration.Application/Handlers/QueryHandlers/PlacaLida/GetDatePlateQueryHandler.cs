

using Alphadigi_migration.Application.Queries.PlacaLida;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.PlacaLida;

public class GetDatePlateQueryHandler : IRequestHandler<GetDatePlateQuery, 
                                                        List<Domain.EntitiesNew.PlacaLida>>
{
    private readonly IPlacaLidaRepository _repository;
    private readonly ILogger<GetDatePlateQueryHandler> _logger;

    public GetDatePlateQueryHandler(IPlacaLidaRepository repository, 
                                    ILogger<GetDatePlateQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Domain.EntitiesNew.PlacaLida>> Handle(GetDatePlateQuery request, 
                                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando placas lidas: {DataInicio} a {DataFim}", request.DataInicio, request.DataFim);

        var filter = new Domain.DTOs.PlacaLidas.LogGetDatePlateDTO
        {
            Search = request.Placa ?? string.Empty,
            Date = FormatDateRange(request.DataInicio, request.DataFim),
            Page = 1, // Default
            PageSize = 1000 // Número grande para evitar paginação
        };

        if (request.AlphadigiId.HasValue)
        {
            filter.Search += $" alphadigi:{request.AlphadigiId}";
        }

        if (request.AreaId.HasValue)
        {
            filter.Search += $" area:{request.AreaId}";
        }

        if (request.Liberado.HasValue)
        {
            filter.Search += $" liberado:{(request.Liberado.Value ? "sim" : "nao")}";
        }

        if (request.Processado.HasValue)
        {
            filter.Search += $" processado:{(request.Processado.Value ? "sim" : "nao")}";
        }

        return await _repository.GetDatePlateAsync(filter);
    
    }
    private string FormatDateRange(DateTime dataInicio, DateTime dataFim)
    {
      
        return $"{dataInicio:yyyy-MM-dd}:{dataFim:yyyy-MM-dd}";
    }
}
    