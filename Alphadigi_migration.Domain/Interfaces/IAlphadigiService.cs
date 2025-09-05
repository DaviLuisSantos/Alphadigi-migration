
using Alphadigi_migration.Domain.DTOs.Alphadigi;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAlphadigiService
{
    Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> GetOrCreate(string ip);
    Task<bool> UpdateLastPlate(Alphadigi_migration.Domain.EntitiesNew.Alphadigi camera, string plate, DateTime timestamp);
    Task<bool> SyncAlphadigi();
    Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> Get(Guid ip);
    Task<List<Alphadigi_migration.Domain.EntitiesNew.Alphadigi>> GetAll();
    Task<bool> Update(UpdateAlphadigiDTO camera);
    Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> Update(Alphadigi_migration.Domain.EntitiesNew.Alphadigi camera);
    Task<bool> UpdateStage(string stage);
    Task<bool> Delete(int id);
    Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO);
}
