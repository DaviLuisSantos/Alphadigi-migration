namespace Alphadigi_migration.Services
{
    public class MonitorAcessoLinear
    {
        private readonly IVeiculoService _veiculoService;

        public MonitorAcessoLinear(IVeiculoService veiculoService)
        {
            _veiculoService = veiculoService;
        }

        public async Task<bool> DadosVeiculo()
        {
            return true;
        }
    }
}
