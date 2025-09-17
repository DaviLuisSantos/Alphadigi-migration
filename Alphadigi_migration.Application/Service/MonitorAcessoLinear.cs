using Alphadigi_migration.Domain.DTOs.MonitorAcessoLinear;
using Alphadigi_migration.Domain.DTOs.Veiculos;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Service;

public class MonitorAcessoLinear : IMonitorAcessoLinear
{
    private readonly IVeiculoService _veiculoService;
    private readonly IUdpBroadcastService _udpBroadcastService;
    private readonly IUnidadeService _unidadeService;
    private readonly ILogger<MonitorAcessoLinear> _logger;

    public MonitorAcessoLinear(
        IVeiculoService veiculoService,
        IUdpBroadcastService udpBroadcastService,
        IUnidadeService unidadeService,
        ILogger<MonitorAcessoLinear> logger)
    {
        _veiculoService = veiculoService;
        _udpBroadcastService = udpBroadcastService;
        _unidadeService = unidadeService;
        _logger = logger;
    }

    public async Task<bool> DadosVeiculo(DadosVeiculoMonitorDTO dados)
    {
        try
        {
            _logger.LogInformation("Processando dados do veículo para monitor: {Placa}", dados.Placa);

            // 1️⃣ Preparar dados do veículo
            string dadosVeiculoStr = $"{dados.Modelo ?? "INDEFINIDO"} - {dados.Marca ?? "INDEFINIDO"} - {dados.Cor ?? "INDEFINIDO"}";
            string acesso = dados.Acesso;
            string ip = dados.Ip;
          

            // 2️⃣ Obter unidade e vagas
            string unidade = await ObterUnidade(dados.Unidade);
            string vagas = await ObterVagas(dados.Unidade);

            // 3️⃣ Enviar DTO de dados do veículo
            await SendDadosVeiculo(dadosVeiculoStr, acesso, ip, vagas);

            // 4️⃣ Enviar DTO de lista de veículos
            await SendListaVeiculo(dadosVeiculoStr, dados.Placa, dados.HoraAcesso, acesso, unidade, ip);

            _logger.LogInformation("Dados enviados para monitor com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar dados do veículo para monitor");
            return false;
        }
    }

    private async Task<string> ObterUnidade(string unidade)
    {
        if (string.IsNullOrEmpty(unidade) || unidade.Equals("SEM UNIDADE", StringComparison.OrdinalIgnoreCase))
            return "NÃO CADASTRADO";

        return unidade;
    }

    private async Task<string> ObterVagas(string unidade)
    {
        if (unidade == "NÃO CADASTRADO") return "NÃO CADASTRADO";

        try
        {
            var unidadeEntidade = await _unidadeService.GetUnidadeByNome(unidade);
            if (unidadeEntidade == null) return "NÃO CADASTRADO";

            var vagasTotal = await _unidadeService.GetUnidadeInfo(unidadeEntidade.Id);
            return $"{vagasTotal.VagasOcupadasMoradores} / {vagasTotal.NumVagas}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações de vagas da unidade");
            return "NÃO CADASTRADO";
        }
    }

    private async Task SendDadosVeiculo(string dadosVeiculo,  
                                        string acesso, 
                                        string ip, 
                                        string vagas)
    {


        var envioUdp = new UdpDadosVeiculoMonitorDTO
        {
            DadosVeiculo = dadosVeiculo,
            Obs = acesso,
            TotalVagas = vagas,
            CorAviso = MonitorColor(acesso),
            AvisoVisible = true
        };

        await _udpBroadcastService.SendAsync(envioUdp, ip, true);
    }

    private async Task SendListaVeiculo(string dadosVeiculo, 
                                        string placa, 
                                        DateTime horaAcesso, 
                                        string acesso, 
                                        string unidade, 
                                        string ip)
    {
        var envioUdp = new UdpListaVeiculoMonitorDTO
        {
            DadosVeiculo = dadosVeiculo,
            Placa = placa,
            DataHora = horaAcesso.ToString("HH:mm:ss"),
            Obs = acesso,
            Unidade = unidade,
            PlaPlacaLidaDiretoLPRca = "",
          
        };

        await _udpBroadcastService.SendAsync(envioUdp, ip, false);
    }

    private string MonitorColor(string acesso)
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
