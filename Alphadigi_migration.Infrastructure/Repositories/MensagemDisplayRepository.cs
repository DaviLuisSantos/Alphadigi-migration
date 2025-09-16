using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class MensagemDisplayRepository :IMensagemDisplayRepository
{
    private readonly AppDbContextSqlite _contextSqlite;
    public MensagemDisplayRepository(AppDbContextSqlite contextSqlite)
    {
        _contextSqlite = contextSqlite;
    }

    public async Task<bool> SaveMensagemDisplayAsync(MensagemDisplay mensagem)
    {
        _contextSqlite.MensagemDisplay.Add(mensagem);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

    public async Task<MensagemDisplay> FindLastMensagemAsync(string placa, 
                                                             string mensagem, 
                                                             int alphadigiId)
    {
        try
        {
            var placaString = placa;

            var mensagemDisplay = await _contextSqlite.MensagemDisplay
                .Where(x => x.Placa == placa)

           
            .Where(x => x.Mensagem == mensagem)
            .Where(x => x.DataHora.AddSeconds(12) > DateTime.Now)
            .Where(x => x.AlphadigiId == alphadigiId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
            return mensagemDisplay;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<MensagemDisplay> FindLastCamMensagemAsync(int alphadigiId)
    {
        try
        {
            var mensagemDisplay = await _contextSqlite.MensagemDisplay
                .Where(x => x.AlphadigiId == alphadigiId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            return mensagemDisplay;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
