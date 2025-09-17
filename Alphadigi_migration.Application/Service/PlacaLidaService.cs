using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.DTOs.PlacaLidas;

namespace Alphadigi_migration.Application.Service;

public class PlacaLidaService : IPlacaLidaService
{
    private readonly IPlacaLidaRepository _placaLidaRepository;

    public PlacaLidaService(IPlacaLidaRepository placaLidaRepository)
    {
        _placaLidaRepository = placaLidaRepository;
    }

    public async Task<bool> SavePlacaLida(PlacaLida placaLida)
    {
        return await _placaLidaRepository.SavePlacaLidaAsync(placaLida);
    }

    public async Task<bool> UpdatePlacaLida(PlacaLida placaLida)
    {
        return await _placaLidaRepository.UpdatePlacaLidaAsync(placaLida);
    }

    public async Task<List<PlacaLida>> GetDatePlate(LogGetDatePlateDTO logPayload)
    {
        return await _placaLidaRepository.GetDatePlateAsync(logPayload);
    }
}