using Alphadigi_migration.Models;

namespace Alphadigi_migration.Services;

public interface IAreaService
{
    Task<Area> GetById(int id);
}
