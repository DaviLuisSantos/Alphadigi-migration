using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; //Importe o logger

namespace Alphadigi_migration.Services;

public class VeiculoService : IVeiculoService
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VeiculoService> _logger; //Adicione o logger
    public VeiculoService(AppDbContextFirebird context, ILogger<VeiculoService> logger) //Injeta o logger
    {
        _contextFirebird = context;
        _logger = logger; //Salva o logger
    }
    public async Task<List<Veiculo>> GetVeiculos()
    {
        _logger.LogInformation("GetVeiculos chamado"); //Adicione logging
        return await _contextFirebird.Veiculos.ToListAsync();
    }
    public async Task<List<IVeiculoService.VeiculoInfo>> GetVeiculosSend(int lastId) //Implementa a interface
    {
        _logger.LogInformation($"GetVeiculosSend chamado com lastId: {lastId}"); //Adicione logging
        return await _contextFirebird.Veiculos
            .Where(v => v.Id > lastId)
            .OrderBy(v => v.Id)
            .Take(1000)
            .Select(v => new IVeiculoService.VeiculoInfo //Use a classe da interface
            {
                Id = v.Id,
                Placa = v.Placa
            })
            .ToListAsync();
    }

    public async Task<Veiculo> getByPlate(string plate)
    {
        _logger.LogInformation($"getByPlate chamado com placa: {plate}"); // Adicione logging
        var resultado = _contextFirebird.Veiculos
                .AsEnumerable() // Switch to client-side evaluation
                .Select(v => new
                {
                    v,
                    A = v.Placa
                        .Take(7) // Pega os 7 primeiros caracteres da placa
                        .Select((c, index) => index < plate.Length && c == plate[index] ? 1 : 0) // Compara cada caractere
                        .Sum() // Soma os resultados
                })
                .Where(v => v.A >= 6) // Filtra onde A >= 6
                .OrderByDescending(v => v.A) // Ordena por A em ordem decrescente
                .Select(v => v.v) // Seleciona o objeto Veiculo completo
                .ToList();
        if (resultado.Count == 0)
        {
            return null;
        }
        return resultado.First();
    }
}