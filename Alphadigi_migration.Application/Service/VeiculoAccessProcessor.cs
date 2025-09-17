using Alphadigi_migration.Domain.DTOs;
using Alphadigi_migration.Domain.DTOs.Veiculos;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Service;

public class VeiculoAccessProcessor : IVeiculoAccessProcessor
{
    private readonly IVeiculoService _veiculoService;
    private readonly IUnidadeService _unidadeService;
    private readonly IMonitorAcessoLinear _monitorAcessoLinear;
    private readonly ILogger<VeiculoAccessProcessor> _logger;

    public VeiculoAccessProcessor(
        IVeiculoService veiculoService,
        IUnidadeService unidadeService,
        IMonitorAcessoLinear monitorAcessoLinear,
        ILogger<VeiculoAccessProcessor> logger)
    {
        _veiculoService = veiculoService;
        _unidadeService = unidadeService;
        _monitorAcessoLinear = monitorAcessoLinear;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(
        Veiculo veiculo,
        Alphadigi alphadigi, 
        DateTime timestamp)
    {
        _logger.LogInformation($"Iniciando ProcessVeiculoAccessAsync");
        string acesso = "";
        bool shouldReturn = false;

        try
        {
            

            return (shouldReturn, acesso);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao processar acesso do veículo.");
            throw;
        }
    }

    private async Task<bool> SendMonitorAcessoLinear(Veiculo veiculo, 
                                                     string ipCamera, 
                                                     string acesso, 
                                                     DateTime timestamp)
    {
        var monitorAcesso = new DadosVeiculoMonitorDTO
        {
            Placa = veiculo.Placa,
            Ip = ipCamera,
            Acesso = acesso,
            HoraAcesso = timestamp
        };
        return await _monitorAcessoLinear.DadosVeiculo(monitorAcesso);
    }

    private async Task<bool> SendUpdateLastAccess(string ipCamera, 
                                                  int idVeiculo, 
                                                  DateTime timestamp)
    {
        var lastAccess = new LastAcessUpdateVeiculoDTO
        {
            IdVeiculo = idVeiculo,
            IpCamera = ipCamera,
            TimeAccess = timestamp
        };

        return await _veiculoService.UpdateLastAccess(lastAccess);
    }
}