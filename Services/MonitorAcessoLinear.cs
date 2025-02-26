using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.MonitorAcessoLinear;

namespace Alphadigi_migration.Services;

public class MonitorAcessoLinear
{
    private readonly IVeiculoService _veiculoService;
    private readonly UdpBroadcastService _udpBroadcastService;
    private readonly UnidadeService _unidadeService;

    public MonitorAcessoLinear(IVeiculoService veiculoService, UdpBroadcastService udpBroadcast,UnidadeService unidade)
    {
        _veiculoService = veiculoService;
        _udpBroadcastService = udpBroadcast;
        _unidadeService = unidade;
    }

    public async Task<bool> DadosVeiculo(DadosVeiculoMonitorDTO dados)
    {
        var dadosVeiculo = _veiculoService.prepareVeiculoDataString(dados.Veiculo);
        string ipCamera, acesso, unidade,vagas;
        ipCamera = dados.Ip;
        acesso = dados.Acesso;
        unidade = dados.Veiculo.UnidadeNavigation?.Nome != null ? dados.Veiculo.UnidadeNavigation.Nome : "NÃO CADASTRADO";
        

        if (unidade == "NÃO CADASTRADO")
        {
            vagas = "NÃO CADASTRADO";
        }
        else
        {
            var vagasTotal = await _unidadeService.GetUnidadeInfo(dados.Veiculo.UnidadeNavigation.Id);

            vagas = $"{vagasTotal.VagasOcupadasMoradores} / {vagasTotal.NumVagas}"; 
        }

        await SendDadosVeiculo(dadosVeiculo, acesso, ipCamera, vagas);

        await sendListaVeiculo(dadosVeiculo, dados.Veiculo.Placa, dados.HoraAcesso, acesso, unidade, ipCamera);

        return true;
    }

    private async Task<bool> SendDadosVeiculo(string veiculo, string acesso, string ip, string vagas)
    {
        var envioUdp = new UdpDadosVeiculoMonitorDTO
        {
            TotalVagas = vagas,
            CorAviso = MonitorColor(acesso),
            AvisoVisible = true,
            DadosVeiculo = veiculo,
            Obs = acesso
        };
        await _udpBroadcastService.SendAsync(envioUdp, ip, true);
        return true;
    }

    private async Task<bool> sendListaVeiculo(string dadosVeiculo, string placa, DateTime HoraAcesso, string acesso, string nomeUnidade, string ip)
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
        await _udpBroadcastService.SendAsync(envioUdp, ip, false);
        return true;
    }

    public string MonitorColor(string acesso)
    {
        return acesso switch
        {
            "NÃO CADASTRADO" => "clred",
            "LIBERADO" => "clgreen",
            "S/VG" => "clyellow",
            "CADASTRADO" => "clgreen",
            _ => "clpurple",
        };
    }
}
