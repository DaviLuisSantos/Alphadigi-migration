using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Alphadigi_migration.Application.Service;

public interface IVeiculoService
{
    Task<List<Veiculo>> GetVeiculos();
    Task<List<Domain.DTOs.Veiculos.VeiculoInfoSendAlphadigi>> GetVeiculosSend(int lastId);
    Task<Veiculo> GetByPlate(string plate);
    Task<bool> UpdateVagaVeiculo(int id, bool dentro);
    Task<bool> UpdateLastAccess(Domain.DTOs.Veiculos.LastAcessUpdateVeiculoDTO lastAccess);
    string PrepareVeiculoDataString(Veiculo veiculo);

    
}

public class VeiculoService : IVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ILogger<VeiculoService> _logger;
    private readonly PlateComparisonSettings _plateSettings;

    public VeiculoService(
        IVeiculoRepository veiculoRepository,
        ILogger<VeiculoService> logger,
        IOptions<PlateComparisonSettings> plateSettings)
    {
        _veiculoRepository = veiculoRepository;
        _logger = logger;
        _plateSettings = plateSettings.Value;
    }

    public async Task<List<Veiculo>> GetVeiculos()
    {
        _logger.LogInformation("GetVeiculos chamado");
        return await _veiculoRepository.GetVeiculosAsync();
    }

    public async Task<List<Domain.DTOs.Veiculos.VeiculoInfoSendAlphadigi>> GetVeiculosSend(int lastId)
    {
        _logger.LogInformation($"GetVeiculosSend chamado com lastId: {lastId}");
        return await _veiculoRepository.GetVeiculosSendAsync(lastId);
    }

    public async Task<Veiculo> GetByPlate(string plate)
    {
        _logger.LogInformation($"GetByPlate chamado com placa: {plate}");
        int minMatching = _plateSettings.MinMatchingCharacters;
        return await _veiculoRepository.GetByPlateAsync(plate, minMatching);
    }

    public async Task<bool> UpdateVagaVeiculo(int id, bool dentro)
    {
        _logger.LogInformation($"UpdateVagaVeiculo chamado com id: {id} e dentro: {dentro}");
        return await _veiculoRepository.UpdateVagaVeiculoAsync(id, dentro);
    }

    public async Task<bool> UpdateLastAccess(Domain.DTOs.Veiculos.LastAcessUpdateVeiculoDTO lastAccess)
    {
        _logger.LogInformation($"UpdateLastAccess chamado com id: {lastAccess.IdVeiculo}");
        return await _veiculoRepository.UpdateLastAccessAsync(lastAccess);
    }

    public string PrepareVeiculoDataString(Veiculo veiculo)
    {
        if (veiculo.Marca == null && veiculo.Modelo == null && veiculo.Cor == null)
        {
            return "INDEFINIDO";
        }
        return $"{veiculo.Modelo} - {veiculo.Marca} - {veiculo.Cor}";
    }
  

}