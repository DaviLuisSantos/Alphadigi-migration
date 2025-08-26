
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Alphadigi_migration.Models;
using System.Numerics;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class MensagemDisplayRepository : IMensagemDisplayRepository
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

    public async Task<MensagemDisplay> FindLastMensagemAsync(FindLastMessage termo)
    {
        try
        {
            var mensagemDisplay =  _contextSqlite.MensagemDisplay
            .Where(x => x.Placa == termo.Placa)
            .Where(x => x.Mensagem == termo.Mensagem)
            .Where(x => x.DataHora.AddSeconds(12) > DateTime.Now)
            .Where(x => x.AlphadigiId == termo.AlphadigiId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();
            return mensagemDisplay ?? new MensagemDisplay();
        }
        catch (Exception ex)
        {
            return new MensagemDisplay();
        }
    }

    public async Task<MensagemDisplay> FindLastCamMensagemAsync(int alphadigiId)
    {
        try
        {
            var mensagemDisplay = _contextSqlite.MensagemDisplay
                .Where(x => x.AlphadigiId == alphadigiId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            return mensagemDisplay ?? new MensagemDisplay();
        }
        catch (Exception ex)
        {
            return new MensagemDisplay();
        }
    }
}
