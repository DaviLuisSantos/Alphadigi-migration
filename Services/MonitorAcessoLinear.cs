using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.MonitorAcessoLinear;

namespace Alphadigi_migration.Services;

public class MonitorAcessoLinear
{
    private readonly IVeiculoService _veiculoService;

    public MonitorAcessoLinear(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    public async Task<bool> DadosVeiculo(DadosVeiculoMonitorDTO dados)
    {
        var envioUdp = new UdpDadosVeiculoMonitorDTO
        {
            TotalVagas = "1 / 2",
            CorAviso = MonitorColor(dados.Acesso),
        };
        return true;
    }

    public string MonitorColor(string acesso)
    {
        return acesso switch
        {
            "NÃO CADASTRADO" => "clpurple",
            "LIBERADO" => "clgreen",
            "S/VG" => "clyellow",
            "CADASTRADO" => "clgreen",
            _ => "clpurple",
        };
    }
}
