using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Models;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Service;
public class AlphadigiService : IAlphadigiService
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<AlphadigiService> _logger;

    public AlphadigiService(IAlphadigiRepository repository, ILogger<AlphadigiService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<bool> SyncAlphadigi() => _repository.SyncAlphadigi();

    public Task<List<Alphadigi_migration.Domain.Entities.Alphadigi>> GetAll() => _repository.GetAll();

    public Task<Alphadigi_migration.Domain.Entities.Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO) => _repository.Create(alphadigiDTO);

    public Task<bool> Update(UpdateAlphadigiDTO alphadigi) => _repository.Update(alphadigi);
    public Task<Alphadigi_migration.Domain.Entities.Alphadigi> Update(Alphadigi_migration.Domain.Entities.Alphadigi camera) => _repository.Update(camera);

    public Task<Alphadigi_migration.Domain.Entities.Alphadigi> Get(string ip) => _repository.Get(ip);

    public Task<Alphadigi_migration.Domain.Entities.Alphadigi> GetOrCreate(string ip) => _repository.GetOrCreate(ip);

    public Task<bool> UpdateLastPlate(Alphadigi_migration.Domain.Entities.Alphadigi camera, string plate, DateTime timestamp) => _repository.UpdateLastPlate(camera, plate, timestamp);

    public Task<bool> UpdateStage(string stage) => _repository.UpdateStage(stage);

    public Task<bool> Delete(int id) => _repository.Delete(id);
}

