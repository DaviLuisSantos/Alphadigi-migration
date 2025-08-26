using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAreaService
{
    Task<Area> GetById(int id);
    Task<bool> SyncAreas();
}
