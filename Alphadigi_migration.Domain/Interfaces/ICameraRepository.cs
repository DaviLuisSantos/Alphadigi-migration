using Alphadigi_migration.Domain.EntitiesNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface ICameraRepository
{
   
  
   
        
        Task<Guid> AddAsync(Domain.EntitiesNew.Camera camera);
        Task<bool> UpdateAsync(Domain.EntitiesNew.Camera camera);
        Task<bool> DeleteAsync(Guid id);
        Task<Domain.EntitiesNew.Camera> GetByIdAsync(Guid id);
        Task<Domain.EntitiesNew.Camera> GetByIpAsync(string ip);
        Task<List<Domain.EntitiesNew.Camera>> GetAllAsync();
        Task<List<Domain.EntitiesNew.Camera>> GetByAreaIdAsync(int areaId);

   
    

}
