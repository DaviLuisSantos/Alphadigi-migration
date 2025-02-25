using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.MonitorAcessoLinear;

namespace Alphadigi_migration.Services;

public class MonitorAcessoLinear
{
    private readonly IVeiculoService _veiculoService;
    private readonly UdpBroadcastService _udpBroadcastService;

    public MonitorAcessoLinear(IVeiculoService veiculoService, UdpBroadcastService udpBroadcast)
    {
        _veiculoService = veiculoService;
        _udpBroadcastService = udpBroadcast;
    }

    public async Task<bool> DadosVeiculo(DadosVeiculoMonitorDTO dados)
    {
        var dadosVeiculo = _veiculoService.prepareVeiculoDataString(dados.Veiculo);
        string ipCamera, acesso;
        ipCamera = dados.Ip;
        acesso = dados.Acesso;

        await SendDadosVeiculo(dadosVeiculo, acesso, ipCamera);

        await sendListaVeiculo(dadosVeiculo, dados.Veiculo.Placa, dados.HoraAcesso, acesso, dados.Veiculo.UnidadeNavigation.Nome, ipCamera); 

        return true;
    }

    private async Task<bool> SendDadosVeiculo(string veiculo, string acesso,string ip)
    {
        var envioUdp = new UdpDadosVeiculoMonitorDTO
        {
            TotalVagas = "1 / 2",
            CorAviso = MonitorColor(acesso),
            AvisoVisible = true,
            DadosVeiculo = veiculo,
            Obs = acesso
        };
        await _udpBroadcastService.SendAsync(envioUdp,ip,true);
        return true;
    }

    private async Task<bool> sendListaVeiculo(string dadosVeiculo, string placa, DateTime HoraAcesso, string acesso, string nomeUnidade,string ip)
    {
        string hora = HoraAcesso.ToString("HH:mm:ss");

        var envioUdp = new UdpListaVeiculoMonitorDTO
        {
            DadosVeiculo = dadosVeiculo,
            Placa = placa,
            DataHora = hora,
            Obs = acesso,
            Unidade = nomeUnidade,
            PlaPlacaLidaDiretoLPRca = ""

        };
        await _udpBroadcastService.SendAsync(envioUdp,ip,false);
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
