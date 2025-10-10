using Alphadigi_migration.Domain.EntitiesNew;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IVisitanteRepository
{
    Task<Visitante> ObterPorPlacaAsync(string placa);
    Task<bool> PlacaEstaAutorizadaAsync(string placa);
    Task<IEnumerable<Visitante>> ObterVisitantesDoDiaAsync();
    Task<IEnumerable<Visitante>> ObterVisitantesDentroDoCondominioAsync();

    Task<bool> ExcluirAsync(int id);
    Task<Visitante> ObterPorIdAsync(int id);
    Task<bool> AtualizarAsync(Visitante visitante);
    Task<IEnumerable<Visitante>> ObterVisitantesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Visitante>> ObterVisitantesComEntradaSemSaidaAsync();
    Task<IEnumerable<Visitante>> ObterVisitantesAgendadosParaDataAsync(DateTime data);
}