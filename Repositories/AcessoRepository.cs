using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Alphadigi_migration.Repositories;

public class AcessoRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    public AcessoRepository(AppDbContextFirebird contextFirebird)
    {
        _contextFirebird = contextFirebird;
    }
    public async Task<bool> SaveAcesso(Acesso acesso)
    {
        _contextFirebird.Acesso.Add(acesso);
        await _contextFirebird.SaveChangesAsync();
        return true;
    }
    public async Task<Acesso?> VerifyAntiPassback(Veiculo veiculo, DateTime? timestamp)
    {
        var ultimoAcesso = await _contextFirebird.Acesso
            .Where(a => a.Placa == veiculo.Placa && a.DataHora >= timestamp)
            .OrderByDescending(a => a.DataHora)
            .FirstOrDefaultAsync();

        return ultimoAcesso;
    }
}
