using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.DTO.Veiculo;

namespace Alphadigi_migration.Services;

public class VeiculoService : IVeiculoService
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VeiculoService> _logger;

    public VeiculoService(AppDbContextFirebird context, ILogger<VeiculoService> logger)
    {
        _contextFirebird = context;
        _logger = logger;
    }

    public async Task<List<Veiculo>> GetVeiculos()
    {
        _logger.LogInformation("GetVeiculos chamado");
        return await _contextFirebird.Veiculo.ToListAsync();
    }

    public async Task<List<IVeiculoService.VeiculoInfo>> GetVeiculosSend(int lastId)
    {
        _logger.LogInformation($"GetVeiculosSend chamado com lastId: {lastId}");
        return await _contextFirebird.Veiculo
            .Where(v => v.Id > lastId)
            .OrderBy(v => v.Id)
            .Take(1000)
            .Select(v => new IVeiculoService.VeiculoInfo
            {
                Id = v.Id,
                Placa = v.Placa
            })
            .ToListAsync();
    }

    public async Task<Veiculo> getByPlate(string plate)
    {
        _logger.LogInformation($"getByPlate chamado com placa: {plate}");
        var resultado = _contextFirebird.Veiculo
            .Include(v => v.UnidadeNavigation)
            .AsEnumerable()
            .Select(v => new
            {
                v,
                A = v.Placa
                    .Take(7)
                    .Select((c, index) => index < plate.Length && c == plate[index] ? 1 : 0)
                    .Sum()
            })
            .Where(v => v.A >= 6)
            .OrderByDescending(v => v.A)
            .Select(v => v.v)
            .ToList();
        if (resultado.Count == 0)
        {
            return null;
        }
        return resultado.First();
    }

    public async Task<bool> UpdateVagaVeiculo(int id, bool dentro)
    {
        _logger.LogInformation($"UpdateVagaVeiculo chamado com id: {id} e dentro: {dentro}");
        try
        {
            var veiculo = await _contextFirebird.Veiculo.FindAsync(id);
            if (veiculo == null)
            {
                return false;
            }
            veiculo.VeiculoDentro = dentro;
            await _contextFirebird.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao atualizar a vaga do veículo");
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> UpdateLastAccess(LastAcessUpdateVeiculoDTO lastAccess)
    {
        var veiculo = await _contextFirebird.Veiculo.FindAsync(lastAccess.IdVeiculo);
        if (veiculo == null)
        {
            return false;
        }
        veiculo.IpCamUltAcesso = lastAccess.IpCamera;
        veiculo.DataHoraUltAcesso = lastAccess.TimeAccess;
        await _contextFirebird.SaveChangesAsync();
        return true;
    }

    public string prepareVeiculoDataString(Veiculo veiculo)
    {
        if (veiculo.Marca == null && veiculo.Modelo == null && veiculo.Cor == null)
        {
            return "INDEFINDO";
        }
        return $"{veiculo.Modelo} - {veiculo.Marca} - {veiculo.Cor}";
    }
}
