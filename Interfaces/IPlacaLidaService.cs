using Alphadigi_migration.DTO.PlacaLida;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Interfaces;

public interface IPlacaLidaService
{
    Task<bool> SavePlacaLida(PlacaLida placaLida);
    Task<bool> UpdatePlacaLida(PlacaLida placaLida);
    Task<List<PlacaLida>> GetDatePlate(LogGetDatePlateDTO logPayload);
}
