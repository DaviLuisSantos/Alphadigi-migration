using Alphadigi_migration.Domain.DTOs.Alphadigi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAlphadigiRepository
{
    Task<Alphadigi_migration.Domain.Entities.Alphadigi> GetOrCreate(string ip);
    Task<bool> UpdateLastPlate(Alphadigi_migration.Domain.Entities.Alphadigi camera, string plate, DateTime timestamp);
    Task<bool> SyncAlphadigi();
    Task<Alphadigi_migration.Domain.Entities.Alphadigi> Get(string ip);
    Task<List<Alphadigi_migration.Domain.Entities.Alphadigi>> GetAll();
    Task<bool> Update(UpdateAlphadigiDTO camera);
    Task<Alphadigi_migration.Domain.Entities.Alphadigi> Update(Alphadigi_migration.Domain.Entities.Alphadigi camera);
    Task<bool> UpdateStage(string stage);
    Task<bool> Delete(int id);
    Task<Alphadigi_migration.Domain.Entities.Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO);
}