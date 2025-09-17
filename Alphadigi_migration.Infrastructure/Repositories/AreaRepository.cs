using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly ILogger<AreaRepository> _logger;

    public AreaRepository(
        AppDbContextFirebird contextFirebird,
        AppDbContextSqlite contextSqlite,
        ILogger<AreaRepository> logger)
    {
        _contextFirebird = contextFirebird;
        _contextSqlite = contextSqlite;
        _logger = logger;
    }

    public async Task<Area> GetByIdAsync(int id)
    {
        _logger.LogInformation("GetByIdAsync chamado para área: {Id}", id);
        return await _contextFirebird.Area.FindAsync(id);
    }

    public async Task<List<Area>> GetAllAsync()
    {
        return await _contextFirebird.Area.ToListAsync();
    }

    public async Task<bool> SyncAreasAsync()
    {
        _logger.LogInformation("SyncAreasAsync chamado");

        try
        {
            var areas = await _contextFirebird.Area.ToListAsync();

            foreach (var area in areas)
            {
                var areaSqlite = await _contextSqlite.Areas.FindAsync(area.Id);
                if (areaSqlite == null)
                {
                    _contextSqlite.Areas.Add(area);
                }
                else
                {
                    areaSqlite.AtualizarNome(area.Nome);
                    _contextSqlite.Areas.Update(areaSqlite);
                }
            }

            await _contextSqlite.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar áreas");
            return false;
        }
    }

    public async Task AddAsync(Area area)
    {
        await _contextSqlite.Areas.AddAsync(area);
        await _contextSqlite.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.EntitiesNew.Area area)
    {
        _contextSqlite.Areas.Update(area);
        await _contextSqlite.SaveChangesAsync();
    }
}