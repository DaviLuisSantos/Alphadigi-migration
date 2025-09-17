using Alphadigi_migration.Application.Queries.Condominio;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Condominio;

public class GetNewCondominioQueryHandler : IRequestHandler<GetNewCondominioQuery, 
                                                            Domain.EntitiesNew.Condominio>
{
    private readonly ICondominioRepository _repository;
    private readonly ILogger<GetNewCondominioQueryHandler> _logger;

    public GetNewCondominioQueryHandler(ICondominioRepository repository, 
                                        ILogger<GetNewCondominioQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Condominio> Handle(GetNewCondominioQuery request, 
                                                      CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando condomínio (nova entidade)");

        // Buscar a entidade antiga
        var condominioAntigo = await _repository.GetFirstAsync();

        if (condominioAntigo == null)
            return null;

        // Converter para a nova entidade
        return new Domain.EntitiesNew.Condominio(
            condominioAntigo.Nome,
            condominioAntigo.Cnpj,
            condominioAntigo.Fantasia);
    }
}
