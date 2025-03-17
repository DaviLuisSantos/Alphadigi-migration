using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public class AcessoService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly IVeiculoService _veiculoService;
    private readonly IAlphadigiService _alphadigi;

    public AcessoService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird, IVeiculoService veiculoService, IAlphadigiService alphadigi)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _veiculoService = veiculoService;
        _alphadigi = alphadigi;
    }

    public async Task<bool> saveVeiculoAcesso(Alphadigi alphadigi, Veiculo veiculo, DateTime timestamp)
    {
        /*
        bool estaNoAntiPassback = await verifyPassBack(veiculo, alphadigi, timestamp);

        if (estaNoAntiPassback)
        {
            return false;
        }
        */
        string local = prepareLocalString(alphadigi);
        string dadosVeiculo = _veiculoService.prepareVeiculoDataString(veiculo);
        string unidade = veiculo.UnidadeNavigation == null || string.IsNullOrEmpty(veiculo.UnidadeNavigation.Nome) ? "NAO CADASTRADO" : veiculo.UnidadeNavigation.Nome;
        var acesso = new Acesso
        {
            Local = local,
            DataHora = timestamp,
            Unidade = unidade,
            Placa = veiculo.Placa,
            DadosVeiculo = dadosVeiculo,
            GrupoNome = ""
        };
        _contextFirebird.Acesso.Add(acesso);
        await _contextFirebird.SaveChangesAsync();
        return true;
    }

    private async Task<bool> verifyPassBack(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp)
    {
        Alphadigi? camUlt = await _alphadigi.Get(veiculo.IpCamUltAcesso);

        string? placa = veiculo.Placa;

        TimeSpan? tempoAntipassback = alphadigi.Area.TempoAntipassbackTimeSpan;

        if (camUlt.AreaId != alphadigi.AreaId)
        {
            return false;
        }

        tempoAntipassback = tempoAntipassback ?? TimeSpan.FromSeconds(10);
        var timeLimit = timestamp - tempoAntipassback;

        var recentAccesses = await _contextFirebird.Acesso
            .Where(a => a.Placa == placa && a.DataHora >= timeLimit)
            .ToListAsync();

        return recentAccesses.Any();
    }

    private string prepareLocalString(Alphadigi alphadigi)
    {
        if (alphadigi == null) { 
        return "Sem local";
        }
        else
        {
            string sentido = alphadigi.Sentido ? "ENTRADA" : "SAIDA";
            return $"{alphadigi.Area.Nome} - {alphadigi.Nome} - {sentido}";
        }
    }

}
