using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Alphadigi_migration.Services;

public interface IAlphadigiService
{
    Task<Alphadigi> GetOrCreate(string ip);
    Task<bool> UpdateLastPlate(Alphadigi camera, string plate, DateTime timestamp);
    Task<bool> SyncAlphadigi();
    Task<Alphadigi> Get(string ip);
    Task<List<Alphadigi>> GetAll();
    Task<bool> Update(UpdateAlphadigiDTO camera);
    Task<bool> UpdateStage(string stage);
    Task<bool> Delete(int id);
    Task<Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO);
}

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

    public Task<List<Alphadigi>> GetAll() => _repository.GetAll();

    public Task<Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO) => _repository.Create(alphadigiDTO);

    public Task<bool> Update(UpdateAlphadigiDTO alphadigi) => _repository.Update(alphadigi);

    public Task<Alphadigi> Get(string ip) => _repository.Get(ip);

    public Task<Alphadigi> GetOrCreate(string ip) => _repository.GetOrCreate(ip);

    public Task<bool> UpdateLastPlate(Alphadigi camera, string plate, DateTime timestamp) => _repository.UpdateLastPlate(camera, plate, timestamp);

    public Task<bool> UpdateStage(string stage) => _repository.UpdateStage(stage);

    public Task<bool> Delete(int id) => _repository.Delete(id);
}
