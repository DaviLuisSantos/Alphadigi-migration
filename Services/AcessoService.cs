﻿using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public class AcessoService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly IVeiculoService _veiculoService;

    public AcessoService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird, IVeiculoService veiculoService)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _veiculoService = veiculoService;
    }

    public async Task<bool> saveVeiculoAcesso(Alphadigi alphadigi, Veiculo veiculo, DateTime timestamp)
    {
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

    private string prepareLocalString(Alphadigi alphadigi) {

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
