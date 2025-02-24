using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; //Importe o logger

namespace Alphadigi_migration.Services;

public class UnidadeService
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
       var unidade = await _contextFirebird.Unidade
            .FindAsync(idUnidade);
        var vagasTotais = unidade.Vagas;
        var vagasOcupadas = await _contextFirebird.Veiculo
            .Where(v => v.Unidade == unidade.Nome && v.VeiculoDentro)
            .CountAsync();

        var retorno = new QueryResult
        {
            NumVagas = vagasTotais,
            VagasOcupadasMoradores = vagasOcupadas
        };

        return retorno;

    }

    // Classe para armazenar o resultado da consulta
    public class QueryResult
    {
        public int NumVagas { get; set; }
        public int? VagasOcupadasVisitantes { get; set; }
        public int VagasOcupadasMoradores { get; set; }
    }
}