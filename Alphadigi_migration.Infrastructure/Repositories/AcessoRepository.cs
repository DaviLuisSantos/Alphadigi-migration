// Alphadigi_migration.Infrastructure/Repositories/AcessoRepository.cs
using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class AcessoRepository : IAcessoRepository
{
    private readonly AppDbContextFirebird _contextFirebird;

    public AcessoRepository(AppDbContextFirebird contextFirebird)
    {
        _contextFirebird = contextFirebird;
    }

    public async Task<bool> SaveAcessoAsync(Acesso acesso)
    {
        _contextFirebird.Acesso.Add(acesso);
        await _contextFirebird.SaveChangesAsync();
        return true;
    }

    public async Task<Acesso?> VerifyAntiPassbackAsync(string placa, DateTime? timestamp)
    {
        var ultimoAcesso = await _contextFirebird.Acesso
            .Where(a => a.Placa == placa && a.DataHora >= timestamp)
            .OrderByDescending(a => a.DataHora)
            .FirstOrDefaultAsync();

        return ultimoAcesso;
    }
}