using Alphadigi_migration.Domain.EntitiesNew;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAreaService
{
    Task<Area> GetById(int id);
    Task<bool> SyncAreas();
}
