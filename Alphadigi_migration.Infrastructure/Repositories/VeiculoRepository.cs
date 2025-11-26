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
        if (string.IsNullOrEmpty(plate) || plate.Length < 7)
        {
            _logger.LogWarning("Placa inválida ou muito curta. Retornando null.");
            return null;
        }

        // ✅ Busca exata primeiro
        var veiculoExato = await _contextFirebird.Veiculo
            .FirstOrDefaultAsync(v => v.Placa.Numero == plate);

        if (veiculoExato != null)
            return veiculoExato;

       
        var primeiros7Chars = plate.Substring(0, 7);

        var veiculo7Chars = await _contextFirebird.Veiculo
            .Where(v => v.Placa.Numero != null &&
                       v.Placa.Numero.Length >= 7 &&
                       v.Placa.Numero.Substring(0, 7) == primeiros7Chars)
            .FirstOrDefaultAsync();

        if (veiculo7Chars != null)
        {
            _logger.LogInformation($"Veículo encontrado (7 primeiros caracteres): {veiculo7Chars.Placa.Numero}");
            return veiculo7Chars;
        }

        // ✅ Busca flexível - pelo menos 7 caracteres iguais em qualquer posição
        var todosVeiculos = await _contextFirebird.Veiculo
            .Where(v => v.Placa.Numero != null && v.Placa.Numero.Length >= 7)
            .AsNoTracking()
            .ToListAsync();

        var veiculoSimilar = todosVeiculos
            .Where(v => CountMatchingCharacters(v.Placa.Numero, plate) >= 7)
            .OrderByDescending(v => CountMatchingCharacters(v.Placa.Numero, plate))
            .FirstOrDefault();

        return veiculoSimilar;
    }

    private int CountMatchingCharacters(string str1, string str2)
    {
        int matches = 0;
        int minLength = Math.Min(str1.Length, str2.Length);

        for (int i = 0; i < minLength; i++)
        {
            if (str1[i] == str2[i])
                matches++;
        }

        return matches;
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