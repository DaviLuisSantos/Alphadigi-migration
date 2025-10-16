using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class VisitanteRepository : IVisitanteRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VisitanteRepository> _logger;

    public VisitanteRepository(
        AppDbContextFirebird contextFirebird,
        ILogger<VisitanteRepository> logger)
    {
        _contextFirebird = contextFirebird;
        _logger = logger;
    }

    public async Task<Visitante> ObterPorPlacaAsync(string placa)
    {
        if (string.IsNullOrEmpty(placa))
        {
            _logger.LogWarning("Placa é null ou vazia. Retornando null.");
            return null;
        }

        _logger.LogInformation("ObterPorPlacaAsync chamado com placa: '{Placa}'", placa);

        try
        {
            var hoje = DateTime.Today;
            var placaNormalizada = NormalizarPlaca(placa);

            // Buscar TODOS os visitantes (solução temporária devido ao problema de conversão)
            var todosVisitantes = await _contextFirebird.Visitantes
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Total de visitantes no banco: {Total}", todosVisitantes.Count);

            // CORREÇÃO: Nova lógica de validação
            var visitanteValido = todosVisitantes
                .Where(v => v.Placa != null && NormalizarPlaca(v.Placa.Numero) == placaNormalizada)
                .Where(v => !v.DataHoraSaida.HasValue) // Que ainda não saiu
                .Where(v =>
                    // Visitante é válido se:
                    // 1. Não tem data agendada (visita espontânea) OU
                    // 2. Tem data agendada para hoje
                    !v.DataVisitaAgendada.HasValue ||
                    v.DataVisitaAgendada.Value.Date == hoje
                )
                .OrderByDescending(v => v.DataVisitaAgendada) // Prioriza os com data mais recente
                .FirstOrDefault();

            if (visitanteValido != null)
            {
                _logger.LogInformation("✅ VISITANTE AUTORIZADO: {Nome} - {Placa}",
                    visitanteValido.Nome, visitanteValido.Placa?.Numero);
                _logger.LogInformation("   Unidade: {Unidade}", visitanteValido.UnidadeDestino);
                _logger.LogInformation("   Data Agendada: {Data}",
                    visitanteValido.DataVisitaAgendada?.ToString("dd/MM/yyyy") ?? "NÃO AGENDADA (visita espontânea)");
                _logger.LogInformation("   Entrada: {Entrada}",
                    visitanteValido.DataHoraEntrada?.ToString("dd/MM/yyyy HH:mm") ?? "NÃO REGISTRADA");
                _logger.LogInformation("   Saída: {Saida}",
                    visitanteValido.DataHoraSaida?.ToString("dd/MM/yyyy HH:mm") ?? "NÃO REGISTRADA");
            }
            else
            {
                _logger.LogInformation("❌ Nenhum visitante válido encontrado para: '{Placa}'", placa);

                // DEBUG: Mostrar por que não foi válido
                var visitanteInvalido = todosVisitantes
                    .Where(v => v.Placa != null && NormalizarPlaca(v.Placa.Numero) == placaNormalizada)
                    .FirstOrDefault();

                if (visitanteInvalido != null)
                {
                    _logger.LogInformation("⚠️  Visitante encontrado mas INVÁLIDO: {Nome}", visitanteInvalido.Nome);
                    _logger.LogInformation("   Motivo: {Motivo}",
                        visitanteInvalido.DataHoraSaida.HasValue ? "JÁ SAIU" :
                        visitanteInvalido.DataVisitaAgendada.HasValue && visitanteInvalido.DataVisitaAgendada.Value.Date != hoje ?
                        $"AGENDADO PARA OUTRA DATA ({visitanteInvalido.DataVisitaAgendada.Value.ToString("dd/MM/yyyy")})" :
                        "OUTRO MOTIVO");
                }
            }

            return visitanteValido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitante por placa: {Placa}", placa);
            throw;
        }
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        _logger.LogInformation("ExcluirAsync chamado para visitante ID: {Id}", id);

        try
        {
            var visitante = await _contextFirebird.Visitantes.FindAsync(id);
            if (visitante == null)
            {
                _logger.LogWarning("Visitante não encontrado para exclusão. ID: {Id}", id);
                return false;
            }

            _contextFirebird.Visitantes.Remove(visitante);
            int rowsAffected = await _contextFirebird.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("✅ Visitante excluído com sucesso. ID: {Id}, Nome: {Nome}",
                    id, visitante.Nome);
                return true;
            }
            else
            {
                _logger.LogWarning("Nenhuma linha afetada na exclusão do visitante ID: {Id}", id);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir visitante ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> PlacaEstaAutorizadaAsync(string placa)
    {
        _logger.LogInformation("PlacaEstaAutorizadaAsync chamado com placa: '{Placa}'", placa);

        try
        {
            var visitante = await ObterPorPlacaAsync(placa);
            var autorizado = visitante != null;

            _logger.LogInformation("Placa {Placa} autorizada: {Autorizado}", placa, autorizado);
            return autorizado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar autorização da placa: {Placa}", placa);
            throw;
        }
    }

    public async Task<IEnumerable<Visitante>> ObterVisitantesDoDiaAsync()
    {
        _logger.LogInformation("ObterVisitantesDoDiaAsync chamado");

        try
        {
            var hoje = DateTime.Today;
            var visitantes = await _contextFirebird.Visitantes
                .Where(v => v.DataVisitaAgendada.HasValue && v.DataVisitaAgendada.Value.Date == hoje)
                .Where(v => !v.DataHoraSaida.HasValue)
                .OrderBy(v => v.DataVisitaAgendada)
                .ToListAsync();

            _logger.LogInformation("Encontrados {Count} visitantes para hoje", visitantes.Count);
            return visitantes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitantes do dia");
            throw;
        }
    }

    public async Task<IEnumerable<Visitante>> ObterVisitantesDentroDoCondominioAsync()
    {
        _logger.LogInformation("ObterVisitantesDentroDoCondominioAsync chamado");

        try
        {
            var visitantes = await _contextFirebird.Visitantes
                .Where(v => v.DataHoraEntrada.HasValue && !v.DataHoraSaida.HasValue)
                .OrderByDescending(v => v.DataHoraEntrada)
                .ToListAsync();

            _logger.LogInformation("Encontrados {Count} visitantes dentro do condomínio", visitantes.Count);
            return visitantes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitantes dentro do condomínio");
            throw;
        }
    }

    public async Task<Visitante> ObterPorIdAsync(int id)
    {
        _logger.LogInformation("ObterPorIdAsync chamado com ID: {Id}", id);

        try
        {
            var visitante = await _contextFirebird.Visitantes
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visitante != null)
            {
                _logger.LogInformation("Visitante encontrado: ID {Id}, Nome: {Nome}", id, visitante.Nome);
            }
            else
            {
                _logger.LogWarning("Visitante não encontrado para o ID: {Id}", id);
            }

            return visitante;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitante por ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(Visitante visitante)
    {
        _logger.LogInformation("AtualizarAsync chamado para visitante ID: {Id}, Nome: {Nome}",
            visitante?.Id, visitante?.Nome);

        try
        {
            if (visitante == null)
            {
                _logger.LogError("Tentativa de atualizar visitante nulo");
                return false;
            }

            _contextFirebird.Visitantes.Update(visitante);
            int rowsAffected = await _contextFirebird.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Visitante atualizado com sucesso. ID: {Id}, Linhas afetadas: {Rows}",
                    visitante.Id, rowsAffected);
                return true;
            }
            else
            {
                _logger.LogWarning("Nenhuma linha foi afetada. Visitante não foi atualizado. ID: {Id}", visitante.Id);
                return false;
            }
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro de banco de dados ao atualizar visitante ID: {Id}", visitante?.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar atualizar visitante ID: {Id}", visitante?.Id);
            throw;
        }
    }

    public async Task<IEnumerable<Visitante>> ObterVisitantesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        _logger.LogInformation("ObterVisitantesPorPeriodoAsync chamado: {DataInicio} a {DataFim}",
            dataInicio.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"));

        try
        {
            var visitantes = await _contextFirebird.Visitantes
                .Where(v => v.DataVisitaAgendada.HasValue &&
                           v.DataVisitaAgendada.Value.Date >= dataInicio.Date &&
                           v.DataVisitaAgendada.Value.Date <= dataFim.Date)
                .OrderByDescending(v => v.DataVisitaAgendada)
                .ToListAsync();

            _logger.LogInformation("Encontrados {Count} visitantes no período", visitantes.Count);
            return visitantes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitantes por período: {DataInicio} a {DataFim}",
                dataInicio.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"));
            throw;
        }
    }

    public async Task<IEnumerable<Visitante>> ObterVisitantesComEntradaSemSaidaAsync()
    {
        _logger.LogInformation("ObterVisitantesComEntradaSemSaidaAsync chamado");

        try
        {
            var visitantes = await _contextFirebird.Visitantes
                .Where(v => v.DataHoraEntrada.HasValue && !v.DataHoraSaida.HasValue)
                .Where(v => v.DataHoraEntrada.Value < DateTime.Now.AddHours(-12))
                .OrderBy(v => v.DataHoraEntrada)
                .ToListAsync();

            _logger.LogInformation("Encontrados {Count} visitantes com entrada sem saída", visitantes.Count);
            return visitantes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitantes com entrada sem saída");
            throw;
        }
    }

    public async Task<IEnumerable<Visitante>> ObterVisitantesAgendadosParaDataAsync(DateTime data)
    {
        _logger.LogInformation("ObterVisitantesAgendadosParaDataAsync chamado para: {Data}",
            data.ToString("dd/MM/yyyy"));

        try
        {
            var visitantes = await _contextFirebird.Visitantes
                .Where(v => v.DataVisitaAgendada.HasValue && v.DataVisitaAgendada.Value.Date == data.Date)
                .OrderBy(v => v.DataVisitaAgendada)
                .ToListAsync();

            _logger.LogInformation("Encontrados {Count} visitantes agendados para {Data}",
                visitantes.Count, data.ToString("dd/MM/yyyy"));
            return visitantes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar visitantes agendados para a data: {Data}",
                data.ToString("dd/MM/yyyy"));
            throw;
        }
    }

    private string NormalizarPlaca(string placa)
    {
        return placa?.Replace("-", "").Replace(" ", "").ToUpper();
    }
}