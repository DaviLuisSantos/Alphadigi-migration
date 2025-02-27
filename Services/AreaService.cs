using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public interface IAreaService
{
    Task<Area> GetById(int id);
    Task<bool> SyncAreas();
}
public class AreaService : IAreaService
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly ILogger<VeiculoService> _logger; //Adicione o logger
    public AreaService(AppDbContextFirebird contextFire, AppDbContextSqlite contextSqlite, ILogger<VeiculoService> logger) //Injeta o logger
    {
        _contextFirebird = contextFire;
        _contextSqlite = contextSqlite;
        _logger = logger; //Salva o logger
    }

    public async Task<Area> GetById(int id)
    {
        _logger.LogInformation("GetAreas chamado"); //Adicione logging
        return await _contextFirebird.Area.FindAsync(id);
    }

    public async Task<bool> SyncAreas()
    {

        _logger.LogInformation("SyncAreas chamado"); //Adicione logging
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
                areaSqlite.Nome = area.Nome;
                _contextSqlite.Areas.Update(areaSqlite);
            }
        }
        await _contextSqlite.SaveChangesAsync();
        return true;

    }
}
