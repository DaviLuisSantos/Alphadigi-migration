using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.ValueObjects;
using Microsoft.Extensions.Logging; 

namespace Alphadigi_migration.Application.Service;

public interface IUnidadeService
{
    Task<QueryResult> GetUnidadeInfo(int idUnidade);

    Task<Unidade> GetUnidadeByNome(string nome);
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


    public async Task<QueryResult> GetUnidadeInfo(int idUnidade)
    {
        return await _unidadeRepository.GetUnidadeInfoAsync(idUnidade);
    }

    public async Task<Unidade> GetUnidadeByNome(string nome)
    {
        return await _unidadeRepository.GetUnidadeByNomeAsync(nome);
    }

}