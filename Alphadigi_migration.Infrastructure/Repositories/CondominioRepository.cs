// Alphadigi_migration.Infrastructure/Repositories/CondominioRepository.cs
using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class CondominioRepository : ICondominioRepository
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<CondominioRepository> _logger;

    public CondominioRepository(
        AppDbContextSqlite contextSqlite,
        AppDbContextFirebird contextFirebird,
        ILogger<CondominioRepository> logger)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _logger = logger;
    }

    public async Task<bool> SyncCondominioAsync()
    {
        try
        {
            var condominio = _contextFirebird.Condominio.FirstOrDefault();
            var condominioSqlite = await _contextSqlite.Condominio.FindAsync(condominio.Id);

            if (condominioSqlite == null)
            {
                Condominio condNew = new Condominio
                {
                    Id = condominio.Id,
                    Nome = condominio.Nome,
                    Cnpj = condominio.Cnpj,
                    Fantasia = condominio.Fantasia
                };
                _contextSqlite.Condominio.Add(condNew);
            }
            else
            {
                condominioSqlite.Nome = condominio.Nome;
                condominioSqlite.Cnpj = condominio.Cnpj;
                condominioSqlite.Fantasia = condominio.Fantasia;
                _contextSqlite.Condominio.Update(condominioSqlite);
            }

            await _contextSqlite.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar condominio");
            return false;
        }
    }

    public async Task<Condominio> GetFirstAsync()
    {
        return await _contextSqlite.Condominio.FirstOrDefaultAsync();
    }

    public async Task<Condominio> GetByIdAsync(int id)
    {
        return await _contextSqlite.Condominio.FindAsync(id);
    }

    public async Task<List<Condominio>> GetAllAsync()
    {
        return await _contextSqlite.Condominio.ToListAsync();
    }

    public async Task AddAsync(Condominio condominio)
    {
        await _contextSqlite.Condominio.AddAsync(condominio);
        await _contextSqlite.SaveChangesAsync();
    }

    public async Task UpdateAsync(Condominio condominio)
    {
        _contextSqlite.Condominio.Update(condominio);
        await _contextSqlite.SaveChangesAsync();
    }
}