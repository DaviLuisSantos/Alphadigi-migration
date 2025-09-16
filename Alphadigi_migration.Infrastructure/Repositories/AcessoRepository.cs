using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 

namespace Alphadigi_migration.Infrastructure.Repositories;

public class AcessoRepository : IAcessoRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<AcessoRepository> _logger; 

    public AcessoRepository(AppDbContextFirebird contextFirebird, ILogger<AcessoRepository> logger)
    {
        _contextFirebird = contextFirebird;
        _logger = logger;
    }

    public async Task<bool> SaveAcessoAsync(Acesso acesso)
    {
        try
        {
            // VALIDAÇÃO CRÍTICA - Corrige os problemas dos logs
            if (acesso == null)
            {
                _logger.LogError("Tentativa de salvar acesso nulo.");
                return false;
            }

            if (string.IsNullOrEmpty(acesso.Placa?.Numero))
            {
                _logger.LogError("Placa é nula ou vazia no objeto acesso.");
                return false;
            }

          
            _logger.LogInformation("Salvando acesso: Placa={Placa}, DataHora={DataHora}, Unidade={Unidade}, TipoAcesso={TipoAcesso}",
                acesso.Placa?.Numero, acesso.DataHora, acesso.Unidade, acesso.Local);

            _contextFirebird.Acesso.Add(acesso);
            int rowsAffected = await _contextFirebird.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Acesso salvo com sucesso. ID: {Id}, Linhas afetadas: {Rows}", acesso.Id, rowsAffected);
                return true;
            }
            else
            {
                _logger.LogWarning("Nenhuma linha foi afetada. Acesso não foi salvo.");
                return false;
            }
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro de banco de dados ao salvar acesso para placa {Placa}", acesso?.Placa?.Numero);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar salvar acesso no banco de dados para placa {Placa}", acesso?.Placa?.Numero);
            throw;
        }
    }


    public async Task<Acesso?> VerifyAntiPassbackAsync(string placa, DateTime? timestamp)
    {
        // 1. Loga a entrada do método e os parâmetros.
        _logger.LogInformation("Iniciando verificação antipassback para: {Placa} com timestamp: {Timestamp}", placa, timestamp);

        try
        {
            // 2. Executa a busca no banco de dados.
            var ultimoAcesso = await _contextFirebird.Acesso
                .Where(a => a.Placa.Numero == placa && a.DataHora >= timestamp)
                .OrderByDescending(a => a.DataHora)
                .FirstOrDefaultAsync();

            // 3. Loga o resultado da busca.
            if (ultimoAcesso != null)
            {
                _logger.LogInformation("Último acesso encontrado para a placa {Placa} em {DataHora}.", placa, ultimoAcesso.DataHora);
            }
            else
            {
                _logger.LogInformation("Nenhum acesso anterior encontrado para a placa {Placa}.", placa);
            }

            // 4. Retorna o resultado.
            return ultimoAcesso;
        }
        catch (Exception ex)
        {
            // 5. Loga qualquer erro que ocorra durante a busca.
            _logger.LogError(ex, "Erro ao verificar anti-passback para a placa {Placa}.", placa);
            throw;
        }
    }
}