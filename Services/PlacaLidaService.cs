using Alphadigi_migration.DTO.PlacaLida;
using Alphadigi_migration.Models;
using Alphadigi_migration.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> SavePlacaLida(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Add(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdatePlacaLida(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Update(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }
    public async Task<List<PlacaLida>> GetDatePlate(LogGetDatePlateDTO logPayload)
    {
        string? placa = logPayload.Search;
        DateTime data = DateTime.ParseExact(logPayload.Date, "yyyy-MM-dd", null);
        int page = logPayload.Page;
        int pageSize = logPayload.PageSize;

        var query = _contextSqlite.PlacaLida.AsQueryable();

        if (!string.IsNullOrEmpty(placa))
        {
            query = query.Where(p => p.Placa.Contains(placa));
        }

        query = query.Where(p => p.DataHora.Date == data.Date);

        // Adiciona a ordenação decrescente por DataHora
        query = query.Include(x => x.Alphadigi).OrderByDescending(p => p.DataHora);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
