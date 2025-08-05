using Alphadigi_migration.Data;
using Alphadigi_migration.DTO.Veiculo;
using Alphadigi_migration.Extensions.Options;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Alphadigi_migration.Services;

public interface IVeiculoService
{
    Task<List<Veiculo>> GetVeiculos();
    Task<List<VeiculoInfoSendAlphadigi>> GetVeiculosSend(int lastId);
    Task<Veiculo> getByPlate(string plate);
    Task<bool> UpdateVagaVeiculo(int id, bool dentro);
    Task<bool> UpdateLastAccess(LastAcessUpdateVeiculoDTO lastAccess);
    string prepareVeiculoDataString(Veiculo veiculo);
}
public class VeiculoService : IVeiculoService
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VeiculoService> _logger;
    private readonly PlateComparisonSettings _plateSettings;

    public VeiculoService(AppDbContextFirebird context, ILogger<VeiculoService> logger, IOptions<PlateComparisonSettings> plateSettings)
    {
        _contextFirebird = context;
        _logger = logger;
        _plateSettings = plateSettings.Value;
    }

    public async Task<List<Veiculo>> GetVeiculos()
    {
        _logger.LogInformation("GetVeiculos chamado");
        return await _contextFirebird.Veiculo.ToListAsync();
    }

    public async Task<List<VeiculoInfoSendAlphadigi>> GetVeiculosSend(int lastId)
    {
        _logger.LogInformation($"GetVeiculosSend chamado com lastId: {lastId}");
        return await _contextFirebird.Veiculo
            .Where(v => v.Id > lastId
                && v.Placa != null
                && v.Placa.Trim().Length == 7
                && !EF.Functions.Like(v.Placa.Trim(), "% %"))
            .OrderBy(v => v.Id)
            .Take(50)
            .Select(v => new VeiculoInfoSendAlphadigi
            {
                Id = v.Id,
                Placa = v.Placa.Trim()
            })
            .ToListAsync();
    }

    public async Task<Veiculo> getByPlate(string plate)
    {
        _logger.LogInformation($"getByPlate chamado com placa: {plate}");
        int minMatching = _plateSettings.MinMatchingCharacters; // Usa o valor configurado

        var resultado = _contextFirebird.Veiculo
            .Include(v => v.UnidadeNavigation)
            .Include(v => v.Rota)
            .AsEnumerable()
            .Select(v => new
            {
                v,
                MatchCount = v.Placa?
                    .Take(7) // Considera apenas os primeiros 7 caracteres
                    .Select((c, index) => index < plate.Length && c == plate[index] ? 1 : 0)
                    .Sum() ?? 0
            })
            .Where(v => v.MatchCount >= minMatching) // Filtra pelo mínimo configurado
            .OrderByDescending(v => v.MatchCount)
            .Select(v => v.v)
            .ToList();

        return resultado.FirstOrDefault();
    }

    public async Task<bool> UpdateVagaVeiculo(int id, bool dentro)
    {
        _logger.LogInformation($"UpdateVagaVeiculo chamado com id: {id} e dentro: {dentro}");
        try
        {
            //Find não rastreia as informações, por isso estávamos tendo problemas. Usamos o FirstOrDefault
            var veiculo = await _contextFirebird.Veiculo.FirstOrDefaultAsync(x => x.Id == id);
            if (veiculo == null)
            {
                _logger.LogWarning($"Veículo com ID {id} não encontrado.");
                return false;
            }
            veiculo.VeiculoDentro = dentro;
            _contextFirebird.Veiculo.Update(veiculo);
            await _contextFirebird.SaveChangesAsync();
            _logger.LogInformation($"Veículo com ID {id} atualizado com sucesso.");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro ao atualizar a vaga do veículo com ID {id}.");
            throw;
        }
    }

    public async Task<bool> UpdateLastAccess(LastAcessUpdateVeiculoDTO lastAccess)
    {
        _logger.LogInformation($"UpdateLastAccess chamado com id: {lastAccess.IdVeiculo}, ip: {lastAccess.IpCamera}, time: {lastAccess.TimeAccess}");
        try
        {
            var veiculo = await _contextFirebird.Veiculo.FindAsync(lastAccess.IdVeiculo);
            if (veiculo == null)
            {
                _logger.LogWarning($"Veículo com ID {lastAccess.IdVeiculo} não encontrado.");
                return false;
            }
            veiculo.IpCamUltAcesso = lastAccess.IpCamera;
            veiculo.DataHoraUltAcesso = lastAccess.TimeAccess;
            _contextFirebird.Veiculo.Update(veiculo);
            await _contextFirebird.SaveChangesAsync();
            _logger.LogInformation($"Veículo com ID {lastAccess.IdVeiculo} atualizado com sucesso.");
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Trata o conflito de concorrência
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues == null)
            {
                // O registro foi excluído por outra transação
                throw new Exception("O veículo foi excluído por outra transação.");
            }
            else
            {
                // Outra transação modificou o registro
                throw new Exception("Outra transação modificou o veículo. Tente novamente.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro ao atualizar o último acesso do veículo com ID {lastAccess.IdVeiculo}.");
            throw;
        }
    }

    public string prepareVeiculoDataString(Veiculo veiculo)
    {
        if (veiculo.Marca == null && veiculo.Modelo == null && veiculo.Cor == null)
        {
            return "INDEFINIDO";
        }
        return $"{veiculo.Modelo} - {veiculo.Marca} - {veiculo.Cor}";
    }
}