using Alphadigi_migration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAreaRepository
{
    Task<Area> GetByIdAsync(int id);
    Task<List<Area>> GetAllAsync();
    Task<bool> SyncAreasAsync();
    Task AddAsync(Area area);
    Task UpdateAsync(Area area);
}