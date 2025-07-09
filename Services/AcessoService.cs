using Alphadigi_migration.Data;
using Alphadigi_migration.Interfaces;
using Alphadigi_migration.Models;
using Alphadigi_migration.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public class AcessoService
{
    private readonly AcessoRepository _acessoRepository;
    private readonly IVeiculoService _veiculoService;
    private readonly IAlphadigiService _alphadigi;

    public AcessoService(AcessoRepository acessoRepository, IVeiculoService veiculoService, IAlphadigiService alphadigi)
    {
        _acessoRepository = acessoRepository;
        _veiculoService = veiculoService;
        _alphadigi = alphadigi;
    }

    public async Task<bool> saveVeiculoAcesso(Alphadigi alphadigi, Veiculo veiculo, DateTime timestamp)
    {
        bool estaNoAntiPassback = false;
        if (veiculo.Id != 0)
        {
            estaNoAntiPassback = await verifyPassBack(veiculo, alphadigi, timestamp);
        }

        if (estaNoAntiPassback)
        {
            return false;
        }

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
        await _acessoRepository.SaveAcesso(acesso);
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
        tempoAntipassback = (tempoAntipassback == null || tempoAntipassback == TimeSpan.Zero) ? TimeSpan.FromSeconds(10) : tempoAntipassback;
        var timeLimit = timestamp - tempoAntipassback;

        var recentAccesses = await _acessoRepository.VerifyAntiPassback(veiculo, timeLimit);

        return recentAccesses != null;
    }

    private string prepareLocalString(Alphadigi alphadigi)
    {
        if (alphadigi == null)
        {
            return "Sem local";
        }
        else
        {
            string sentido = alphadigi.Sentido ? "ENTRADA" : "SAIDA";
            return $"{alphadigi.Area.Nome} - {alphadigi.Nome} - {sentido}";
        }
    }

}
