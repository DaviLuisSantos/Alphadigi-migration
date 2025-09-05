using Alphadigi_migration.Domain.EntitiesNew;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAreaService
{
    Task<Area> GetById(Guid id);
    Task<bool> SyncAreas();
}
