using Alphadigi_migration.Data;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Services
{
    public class AreaService : IAreaService
    {
        private readonly AppDbContextFirebird _contextFirebird;
        private readonly ILogger<VeiculoService> _logger; //Adicione o logger
        public AreaService(AppDbContextFirebird context, ILogger<VeiculoService> logger) //Injeta o logger
        {
            _contextFirebird = context;
            _logger = logger; //Salva o logger
        }

        public async Task<Area> GetById(int id)
        {
            _logger.LogInformation("GetAreas chamado"); //Adicione logging
            return await _contextFirebird.Areas.FindAsync(id);
        }
    }
}
