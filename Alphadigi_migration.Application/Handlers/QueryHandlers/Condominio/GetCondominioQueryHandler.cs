

using Alphadigi_migration.Application.Queries.Condominio;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Condominio;

public class GetCondominioQueryHandler : IRequestHandler<GetCondominioQuery, Domain.EntitiesNew.Condominio>
{
    private readonly ICondominioRepository _repository;
    private readonly ILogger<GetCondominioQueryHandler> _logger;

    public GetCondominioQueryHandler(
        ICondominioRepository repository,
        ILogger<GetCondominioQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<Domain.EntitiesNew.Condominio> Handle(GetCondominioQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando condomínio");
        return await _repository.GetFirstAsync();
    }
}
