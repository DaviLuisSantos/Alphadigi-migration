using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public class CondominioService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<CondominioService> _logger;

    public CondominioService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird, ILogger<CondominioService> logger)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _logger = logger;
    }

    public async Task<bool> SyncCondominio()
    {
        try
        {
            var condominio = _contextFirebird.Condominio.FirstOrDefault();
            var cameraSqlite = await _contextSqlite.Condominio.FindAsync(condominio.Id);
            if (cameraSqlite == null)
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
                cameraSqlite.Nome = condominio.Nome;
                cameraSqlite.Cnpj = condominio.Cnpj;
                cameraSqlite.Fantasia = condominio.Fantasia;
                _contextSqlite.Condominio.Update(cameraSqlite);
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

    public async Task<Condominio> get()
    {
        return await _contextSqlite.Condominio.FirstOrDefaultAsync();
    }
}
