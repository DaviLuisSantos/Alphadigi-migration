using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class GetAlphadigiByIpQueryHandler : IRequestHandler<GetAlphadigiByIpQuery, 
                                                            Domain.EntitiesNew.Alphadigi>
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<GetAlphadigiByIpQueryHandler> _logger;

    public GetAlphadigiByIpQueryHandler(
        IAlphadigiRepository repository,
        ILogger<GetAlphadigiByIpQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Alphadigi> Handle(GetAlphadigiByIpQuery request, 
                                                           CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.GetOrCreate(request.Ip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar Alphadigi por IP");
            throw;
        }
    }
}

