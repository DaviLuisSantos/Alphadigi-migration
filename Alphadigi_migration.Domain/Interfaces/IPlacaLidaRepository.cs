using Alphadigi_migration.Domain.DTOs.PlacaLidas;
using Alphadigi_migration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IPlacaLidaRepository
{
    Task<bool> SavePlacaLidaAsync(PlacaLida placaLida);
    Task<bool> UpdatePlacaLidaAsync(PlacaLida placaLida);
    Task<List<PlacaLida>> GetDatePlateAsync(LogGetDatePlateDTO logPayload);
    Task<PlacaLida> GetByIdAsync(int id);

}
