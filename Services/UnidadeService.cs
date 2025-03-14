﻿using Alphadigi_migration.Data;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; //Importe o logger

namespace Alphadigi_migration.Services;

public interface IUnidadeService
{
    Task<QueryResult> GetUnidadeInfo(int idUnidade);
}
public class QueryResult
{
    public int NumVagas { get; set; }
    public int? VagasOcupadasVisitantes { get; set; }
    public int VagasOcupadasMoradores { get; set; }
}

public class UnidadeService: IUnidadeService
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VeiculoService> _logger; //Adicione o logger
    public UnidadeService(AppDbContextFirebird context, ILogger<VeiculoService> logger) //Injeta o logger
    {
        _contextFirebird = context;
        _logger = logger; //Salva o logger
    }

    public async Task<QueryResult> GetUnidadeInfo(int idUnidade)
    {
        try
        {
            var unidade = await _contextFirebird.Unidade.FindAsync(idUnidade);
            var vagasTotais = unidade.Vagas;
            var vagasOcupadas = _contextFirebird.Veiculo
                .AsEnumerable() // Switch to client-side evaluation
                .Where(v => v.Unidade == unidade.Nome && v.VeiculoDentro)
                .Count();

            var retorno = new QueryResult
            {
                NumVagas = vagasTotais,
                VagasOcupadasMoradores = vagasOcupadas
            };

            return retorno;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar informações da unidade");
            return null;
        }
    }

    // Classe para armazenar o resultado da consulta
}