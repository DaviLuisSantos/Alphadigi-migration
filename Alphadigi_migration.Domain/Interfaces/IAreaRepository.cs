using Alphadigi_migration.Domain.EntitiesNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAreaRepository
{
    Task<Domain.EntitiesNew.Area> GetByIdAsync(Guid id);
    Task<List<Domain.EntitiesNew.Area>> GetAllAsync();
    Task<bool> SyncAreasAsync();
    Task AddAsync(Domain.EntitiesNew.Area area);
    Task UpdateAsync(Domain.EntitiesNew.Area area);
}