using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class CameraRepository : ICameraRepository
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public CameraRepository(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

    public Task AddAsync(Camera camera)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Camera>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Camera>> GetByAreaIdAsync(Guid areaId)
    {
        throw new NotImplementedException();
    }

    public async Task<Camera> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Camera> GetByIpAsync(string ip)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Camera camera)
    {
        throw new NotImplementedException();
    }

    Task<Guid> ICameraRepository.AddAsync(Camera camera)
    {
        throw new NotImplementedException();
    }

    Task<bool> ICameraRepository.DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    Task<bool> ICameraRepository.UpdateAsync(Camera camera)
    {
        throw new NotImplementedException();
    }
}