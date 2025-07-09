using Alphadigi_migration.Models;
using Alphadigi_migration.Data;
using Alphadigi_migration.DTO.MonitorAcessoLinear;
using Alphadigi_migration.DTO.Veiculo;
using Microsoft.AspNetCore.Components.Web;
using Alphadigi_migration.Interfaces;

namespace Alphadigi_migration.Services;

public class VeiculoAccessProcessor : IVeiculoAccessProcessor
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly IVeiculoService _veiculoService;
    private readonly IUnidadeService _unidadeService;
    private readonly MonitorAcessoLinear _monitorAcessoLinear;
    private readonly ILogger<VeiculoAccessProcessor> _logger;

    public VeiculoAccessProcessor(
        AppDbContextFirebird contextFirebird, //Utilize o AppDbContextFirebird Aqui!
        IVeiculoService veiculoService,
        IUnidadeService unidadeService,
        MonitorAcessoLinear monitorAcessoLinear,
        ILogger<VeiculoAccessProcessor> logger)
    {
        _contextFirebird = contextFirebird;
        _veiculoService = veiculoService;
        _unidadeService = unidadeService;
        _monitorAcessoLinear = monitorAcessoLinear;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp)
    {
        _logger.LogInformation($"Iniciando ProcessVeiculoAccessAsync");
        string acesso = "";
        bool shouldReturn = false;
        try
        {
            // 1. Preparação: Extrair informações e definir variáveis
            Area area = alphadigi.Area;
            bool isVisitante = veiculo.Id == 0 || veiculo.Id == null;
            bool isVisita = (area.EntradaVisita || area.SaidaVisita) && isVisitante;
            bool isSaidaSempreAbre = area.SaidaSempreAbre && !alphadigi.Sentido;
            bool isControlaVaga = area.ControlaVaga;

            if (isVisitante)
            {
                _logger.LogInformation($"Processando acesso de visitante.");
                acesso = "NÃO CADASTRADO";
                shouldReturn = false;
            }
            else if (isSaidaSempreAbre)
            {
                _logger.LogInformation($"Processando saída sempre aberta.");
                shouldReturn = true;
                if (veiculo.Id != 0 && !isVisitante)
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                    acesso = "CADASTRADO";
                }
                else
                {
                    acesso = "NÃO CADASTRADO";
                }
            }
            else if (isControlaVaga)
            {
                _logger.LogInformation($"Processando acesso com controle de vaga.");
                if (!alphadigi.Sentido) // Saída
                {
                    _logger.LogInformation($"Processando saída.");
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                    acesso = "CADASTRADO";
                    shouldReturn = true;
                }
                else // Entrada
                {
                    _logger.LogInformation($"Processando entrada.");
                    var vagas = await _unidadeService.GetUnidadeInfo(veiculo.UnidadeNavigation.Id);
                    if (vagas != null && (vagas.NumVagas > vagas.VagasOcupadasMoradores || veiculo.VeiculoDentro))
                    {
                        await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                        acesso = "CADASTRADO";
                        shouldReturn = true;
                    }
                    else
                    {
                        acesso = "S/VG";
                        shouldReturn = false;
                    }
                }
            }
            else // !isControlaVaga
            {
                _logger.LogInformation($"Processando acesso sem controle de vaga.");
                if (!alphadigi.Sentido) // Saída
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                    acesso = "CADASTRADO";
                    shouldReturn = true;
                }
                else // Entrada
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                    acesso = "CADASTRADO";
                    shouldReturn = true;
                }
            }

            // 3. Atualizar informações do veículo (se não for visitante)
            if (!isVisita && veiculo != null && veiculo.Id != 0)
            {
                bool mesmaCamera = veiculo.IpCamUltAcesso == alphadigi.Ip;
                var tempo = timestamp - alphadigi.Area.TempoAntipassbackTimeSpan;
                bool estaNoTempo = veiculo.DataHoraUltAcesso.HasValue && (tempo > veiculo.DataHoraUltAcesso.Value);
                if (!mesmaCamera || estaNoTempo)
                {
                    await sendUpdateLastAccess(alphadigi.Ip, veiculo.Id, timestamp);
                }
                
                await _contextFirebird.SaveChangesAsync();
                _logger.LogInformation($"Informações do veículo {veiculo.Placa} Atualizada.");
            }

            if (alphadigi.UltimaPlaca != veiculo.Placa)
            {
                bool monitor = await sendMonitorAcessoLinear(veiculo, alphadigi.Ip, acesso, timestamp);
            }

            return (shouldReturn, acesso);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao processar acesso do veículo .");
            throw; //Não abafar a exceção, reporte para as camadas superiores!
        }
    }

    private async Task<bool> sendMonitorAcessoLinear(Veiculo veiculo, string ipCamera, string acesso, DateTime timestamp)
    {
        var monitorAcesso = new DadosVeiculoMonitorDTO
        {
            Veiculo = veiculo,
            Ip = ipCamera,
            Acesso = acesso,
            HoraAcesso = timestamp
        };
        return await _monitorAcessoLinear.DadosVeiculo(monitorAcesso);
    }

    private async Task<bool> sendUpdateLastAccess(string ipCamera, int idVeiculo, DateTime timestamp)
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