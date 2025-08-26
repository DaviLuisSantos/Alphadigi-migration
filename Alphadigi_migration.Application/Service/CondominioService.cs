using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Services;

public class CondominioService
{
    private readonly ICondominioRepository _condominioRepository;
    private readonly ILogger<CondominioService> _logger;

    public CondominioService(
        ICondominioRepository condominioRepository,
        ILogger<CondominioService> logger)
    {
        _condominioRepository = condominioRepository;
        _logger = logger;
    }

    public async Task<bool> SyncCondominio()
    {
        return await _condominioRepository.SyncCondominioAsync();
    }

    public async Task<Condominio> Get()
    {
        return await _condominioRepository.GetFirstAsync();
    }
}