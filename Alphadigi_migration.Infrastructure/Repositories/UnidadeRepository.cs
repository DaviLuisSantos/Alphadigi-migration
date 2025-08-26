using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class UnidadeRepository : IUnidadeRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<UnidadeRepository> _logger;

    public UnidadeRepository(
        AppDbContextFirebird context,
        ILogger<UnidadeRepository> logger)
    {
        _contextFirebird = context;
        _logger = logger;
    }

    public async Task<QueryResult> GetUnidadeInfoAsync(int idUnidade)
    {
        try
        {
            var unidade = await _contextFirebird.Unidade.FindAsync(idUnidade);
            var vagasTotais = unidade.Vagas;

            var vagasOcupadas = _contextFirebird.Veiculo
                .AsEnumerable()
                .Where(v => v.Unidade == unidade.Nome && v.VeiculoDentro)
                .Count();

            var retorno = new QueryResult
            {
                NumVagas = vagasTotais,
                VagasOcupadasMoradores = vagasOcupadas
            };

            return retorno;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar informações da unidade");
            return null;
        }
    }

    public async Task<Unidade> GetByIdAsync(int id)
    {
        return await _contextFirebird.Unidade.FindAsync(id);
    }
}