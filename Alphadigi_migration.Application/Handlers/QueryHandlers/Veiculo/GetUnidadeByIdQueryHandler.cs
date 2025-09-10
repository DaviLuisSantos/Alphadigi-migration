using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Veiculo;

public class GetUnidadeByIdQueryHandler : IRequestHandler<GetUnidadeByIdQuery, Unidade>
{
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly ILogger<GetUnidadeByIdQueryHandler> _logger;

    public GetUnidadeByIdQueryHandler(IUnidadeRepository unidadeRepository, ILogger<GetUnidadeByIdQueryHandler> logger)
    {
        _unidadeRepository = unidadeRepository;
        _logger = logger;
    }

    public async Task<Unidade> Handle(GetUnidadeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Buscando unidade por ID: {request.UnidadeId}");

            if (request.UnidadeId <= 0)
            {
                _logger.LogWarning("ID da unidade inválido");
                return null;
            }

            var unidade = await _unidadeRepository.GetByIdAsync(request.UnidadeId);

            if (unidade == null)
            {
                _logger.LogWarning($"Unidade com ID {request.UnidadeId} não encontrada");
                return null;
            }

            _logger.LogInformation($"Unidade encontrada: {unidade.Nome}");
            return unidade;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao buscar unidade com ID {request.UnidadeId}");
            throw;
        }
    }
}