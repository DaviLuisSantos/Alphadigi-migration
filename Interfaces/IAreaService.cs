using Alphadigi_migration.Models;

namespace Alphadigi_migration.Interfaces;

public interface IAreaService
{
    Task<Area> GetById(int id);
    Task<bool> SyncAreas();
}
