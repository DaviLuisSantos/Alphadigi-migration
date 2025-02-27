using Alphadigi_migration.Models;
using Alphadigi_migration.Data;

namespace Alphadigi_migration.Services;

public class PlacaLidaService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public PlacaLidaService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

    public async Task<int> CreatePlacaLida(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Add(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return placaLida.Id;
    }
    public async Task<bool> UpdatePlacaLida(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Update(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }
}
