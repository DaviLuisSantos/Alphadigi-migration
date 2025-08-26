using Alphadigi_migration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface ICameraRepository
{
    Task<Camera> GetByIdAsync(int id);
    Task<List<Camera>> GetAllAsync();
    Task AddAsync(Camera camera);
    Task UpdateAsync(Camera camera);
    Task DeleteAsync(int id);

}
