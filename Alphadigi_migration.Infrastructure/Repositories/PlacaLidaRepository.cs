using Alphadigi_migration.Domain.DTOs.PlacaLidas;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class PlacaLidaRepository : IPlacaLidaRepository
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public PlacaLidaRepository(
        AppDbContextSqlite contextSqlite,
        AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

    public async Task<bool> SavePlacaLidaAsync(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Add(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePlacaLidaAsync(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Update(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

    public async Task<List<PlacaLida>> GetDatePlateAsync(LogGetDatePlateDTO logPayload)
    {
        string? placa = logPayload.Search;
        DateTime data = DateTime.ParseExact(logPayload.Date, "yyyy-MM-dd", null);
        int page = logPayload.Page;
        int pageSize = logPayload.PageSize;

        var query = _contextSqlite.PlacaLida.AsQueryable();

        if (!string.IsNullOrEmpty(placa))
        {
          
           // query = query.Where(p => p.Placa.Contains(placa));

            query = query.Where(p => p.Placa.Numero.Contains(placa));

        }

        query = query.Where(p => p.DataHora.Date == data.Date);

        query = query.Include(x => x.Alphadigi).OrderByDescending(p => p.DataHora);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<PlacaLida> GetByIdAsync(int id)
    {
        return await _contextSqlite.PlacaLida.FindAsync(id);
    }

    public async Task<PlacaLida> AddAsync(PlacaLida placaLida)
    {
        _contextSqlite.PlacaLida.Add(placaLida);
        await _contextSqlite.SaveChangesAsync();
        return placaLida;
    }
}