using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Service;


    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly ILogger<AreaService> _logger;

        public AreaService(
            IAreaRepository areaRepository,
            ILogger<AreaService> logger)
        {
            _areaRepository = areaRepository;
            _logger = logger;
        }

        public async Task<Area> GetById(Guid id)
        {
            _logger.LogInformation("GetById chamado para área: {Id}", id);
            return await _areaRepository.GetByIdAsync(id);
        }

        public async Task<bool> SyncAreas()
        {
            _logger.LogInformation("SyncAreas chamado");
            return await _areaRepository.SyncAreasAsync();
        }
    }

