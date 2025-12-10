using Alphadigi_migration.Domain.DTOs.Veiculos;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class VeiculoRepository : IVeiculoRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VeiculoRepository> _logger;

    public VeiculoRepository(
        AppDbContextFirebird context,
        ILogger<VeiculoRepository> logger)
    {
        _contextFirebird = context;
        _logger = logger;
    }

    public async Task<List<Veiculo>> GetVeiculosAsync()
    {
        _logger.LogInformation("GetVeiculosAsync chamado");
        return await _contextFirebird.Veiculo.ToListAsync();
    }

    public async Task<List<VeiculoInfoSendAlphadigi>> GetVeiculosSendAsync(int lastId)
    {
        _logger.LogInformation($"GetVeiculosSendAsync chamado com lastId: {lastId}");
        return await _contextFirebird.Veiculo
             .Where(v => v.Id > lastId && v.Placa != null)
             .OrderBy(v => v.Id)
             .Take(50)
             .Select(v => new VeiculoInfoSendAlphadigi
             {
                 Id = v.Id,
                 Placa = v.Placa.Numero 
             })
             .ToListAsync();
    }

    public async Task<Veiculo> GetByPlateAsync(string plate)
    {
        if (string.IsNullOrEmpty(plate))
        {
            _logger.LogWarning("Placa é null ou vazia. Retornando null.");
            return null;
        }

        _logger.LogInformation($"GetByPlateAsync chamado com placa: '{plate}'");

        // 1. BUSCA O VALOR QUE JÁ ESTÁ NO BANCO (configurado no AcessoLinear)
        // NÃO precisa de fallback porque o valor sempre existe
        int minMatchingFromDb = await _contextFirebird.Camera
            .Where(c => c.MinMatchingCharacters.HasValue)
            .OrderBy(c => c.Id)
            .Select(c => c.MinMatchingCharacters.Value)
            .FirstOrDefaultAsync();

        if (minMatchingFromDb == 0)
        {
            // Aqui SIM usa o valor padrão da entidade
            minMatchingFromDb = 7; // Ou Camera.ValorPadraoMinMatching
            _logger.LogWarning($"Nenhuma câmera configurada no banco. Usando valor padrão: {minMatchingFromDb}");
        }
        else
        {
            _logger.LogInformation($"Usando valor configurado no AcessoLinear: {minMatchingFromDb}");
        }



        // ✅ Busca exata primeiro
        var veiculoExato = await _contextFirebird.Veiculo
            .FirstOrDefaultAsync(v => v.Placa.Numero == plate);

        if (veiculoExato != null)
        {
            _logger.LogInformation($"Veículo encontrado (busca exata): {veiculoExato.Placa.Numero}");
            return veiculoExato;
        }

        _logger.LogInformation($"Nenhum veículo encontrado por busca exata para: '{plate}'");

        // ✅ Só então usar similaridade
        var veiculos = await _contextFirebird.Veiculo.ToListAsync();

        var resultado = veiculos
            .Select(v => new
            {
                Veiculo = v,
                MatchCount = v.Placa?.Numero?
                    .Take(7)
                    .Select((c, index) => index < plate.Length && c == plate[index] ? 1 : 0)
                    .Sum() ?? 0
            })
            .Where(v => v.MatchCount >= minMatchingFromDb) // Usa valor do banco
            .OrderByDescending(v => v.MatchCount)
            .Select(v => v.Veiculo)
            .ToList();

        var veiculoSimilar = resultado.FirstOrDefault();

        if (veiculoSimilar != null)
        {
            _logger.LogInformation($"Veículo similar encontrado: {veiculoSimilar.Placa.Numero}");
        }
        else
        {
            _logger.LogInformation($"Nenhum veículo similar encontrado para: '{plate}'");
        }

        return veiculoSimilar;
    }
    public async Task<bool> UpdateVagaVeiculoAsync(int id, bool dentro)
    {
        _logger.LogInformation($"UpdateVagaVeiculoAsync chamado com id: {id} e dentro: {dentro}");

        try
        {
            var veiculo = await _contextFirebird.Veiculo.FirstOrDefaultAsync(x => x.Id == id);
            if (veiculo == null)
            {
                _logger.LogWarning($"Veículo com ID {id} não encontrado.");
                return false;
            }
            if (dentro)
            {
                veiculo.EntrarCondominio("Sistema"); 
            }
            else
            {
                veiculo.SairCondominio("Sistema"); 
            }
           
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

    public async Task<bool> UpdateLastAccessAsync(LastAcessUpdateVeiculoDTO lastAccess)
    {
        _logger.LogInformation($"UpdateLastAccessAsync chamado com id: {lastAccess.IdVeiculo}");

        try
        {
            var veiculo = await _contextFirebird.Veiculo.FindAsync(lastAccess.IdVeiculo);
            if (veiculo == null)
            {
                _logger.LogWarning($"Veículo com ID {lastAccess.IdVeiculo} não encontrado.");
                return false;
            }
        
                veiculo.RegistrarAcesso(lastAccess.IpCamera, lastAccess.TimeAccess);
            //veiculo.IpCamUltAcesso = lastAccess.IpCamera;
            //veiculo.DataHoraUltAcesso = lastAccess.TimeAccess;
            _contextFirebird.Veiculo.Update(veiculo);
            await _contextFirebird.SaveChangesAsync();

            _logger.LogInformation($"Veículo com ID {lastAccess.IdVeiculo} atualizado com sucesso.");
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues == null)
            {
                throw new Exception("O veículo foi excluído por outra transação.");
            }
            else
            {
                throw new Exception("Outra transação modificou o veículo. Tente novamente.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro ao atualizar o último acesso do veículo com ID {lastAccess.IdVeiculo}.");
            throw;
        }
    }

    public async Task<Veiculo> GetByIdAsync(int id)
    {
        return await _contextFirebird.Veiculo.FindAsync(id);
    }

   
}