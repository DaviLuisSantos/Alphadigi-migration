using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Interfaces;

public interface IAlphadigiService
{
    Task<Alphadigi> GetOrCreate(string ip);
    Task<bool> UpdateLastPlate(Alphadigi camera, string plate, DateTime timestamp);
    Task<bool> SyncAlphadigi();
    Task<Alphadigi> Get(string ip);
    Task<List<Alphadigi>> GetAll();
    Task<bool> Update(UpdateAlphadigiDTO camera);
    Task<Alphadigi> Update(Alphadigi camera);
    Task<bool> UpdateStage(string stage);
    Task<bool> Delete(int id);
    Task<Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO);
}
