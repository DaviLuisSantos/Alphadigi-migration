using Alphadigi_migration.Domain.DTOs.PlacaLidas;
using Alphadigi_migration.Domain.Entities;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IPlacaLidaService
{
    Task<bool> SavePlacaLida(PlacaLida placaLida);
    Task<bool> UpdatePlacaLida(PlacaLida placaLida);
    Task<List<PlacaLida>> GetDatePlate(LogGetDatePlateDTO logPayload);
   
}
