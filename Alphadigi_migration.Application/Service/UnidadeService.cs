using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.ValueObjects;
using Microsoft.Extensions.Logging; 

namespace Alphadigi_migration.Application.Service;

public interface IUnidadeService
{
    Task<QueryResult> GetUnidadeInfo(Guid idUnidade);
}


public class UnidadeService: IUnidadeService
{
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly ILogger<UnidadeService> _logger;

    public UnidadeService(
        IUnidadeRepository unidadeRepository,
        ILogger<UnidadeService> logger)
    {
        _unidadeRepository = unidadeRepository;
        _logger = logger;
    }


    public async Task<QueryResult> GetUnidadeInfo(Guid idUnidade)
    {
        return await _unidadeRepository.GetUnidadeInfoAsync(idUnidade);
    }

   
}