using Alphadigi_migration.Domain.EntitiesNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface ICondominioRepository
{
    Task<Condominio> GetByIdAsync(int id);
    Task<Condominio> GetFirstAsync();
    Task<List<Condominio>> GetAllAsync();
    Task AddAsync(Condominio condominio);
    Task UpdateAsync(Condominio condominio);
    Task<bool> SyncCondominioAsync();
}