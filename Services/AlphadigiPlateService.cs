using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.DTO.Veiculo;
using Alphadigi_migration.DTO.MonitorAcessoLinear;

namespace Alphadigi_migration.Services;

public class AlphadigiPlateService: IAlphadigiPlateService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;
    private readonly UnidadeService _unidadeService;
    private readonly ILogger<AlphadigiHearthBeatService> _logger;
    private readonly AccessHandlerFactory _accessHandlerFactory;
    private readonly MonitorAcessoLinear _monitorAcessoLinear;


    public AlphadigiPlateService(
            AppDbContextSqlite contextSqlite,
            AppDbContextFirebird contextFirebird,
            IAlphadigiService alphadigiService,
            IVeiculoService veiculoService,
            UnidadeService unidadeService,
            MonitorAcessoLinear monitorAcessoLinear,
            AccessHandlerFactory accessHandlerFactor,
            ILogger<AlphadigiHearthBeatService> logger) // Adicione o logger
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
        _unidadeService = unidadeService;
        _monitorAcessoLinear = monitorAcessoLinear;
        _accessHandlerFactory = accessHandlerFactor;
        _logger = logger; // Salve o logger
    }

    public async Task<Object> ProcessPlate(ProcessPlateDTO plateReaded)
    {
        DateTime timeStamp = DateTime.Now;

        var camera = await _alphadigiService.GetOrCreate(plateReaded.ip);
        if (camera == null)
        {
            throw new Exception("Camera não encontrada");
        }

        var veiculo = await _veiculoService.getByPlate(plateReaded.plate);
        if (veiculo != null)
        {
            if(!await VerifyAntiPassback(veiculo, camera, timeStamp))
                return await handleReturn(plateReaded.plate, plateReaded.ip, liberado: false);
            var retorno = handleVeiculo(veiculo, camera, timeStamp);
        }
        if (veiculo == null)
        {
            return await handleReturn(plateReaded.plate, plateReaded.ip, liberado: true);
        }
        return veiculo;
    }

    public async Task<bool> VerifyAntiPassback(Veiculo veiculo, Alphadigi alphadigi,DateTime timestamp)
    {
        Area? Area, ultimaArea;
        DateTime? ultimoAcesso;
        bool mesmaCamera, mesmaArea,dentroDoPassback;
        mesmaCamera = veiculo.IpCamUltAcesso == alphadigi.Ip;
        Area = alphadigi.Area;

        if (mesmaCamera)
        {
            mesmaArea = true;
        }
        else
        {
            var ultimaCamera = await _alphadigiService.GetOrCreate(veiculo.IpCamUltAcesso);
            ultimaArea = ultimaCamera.Area;
            mesmaArea = ultimaCamera.AreaId == alphadigi.AreaId;
        }

        ultimoAcesso = veiculo.DataHoraUltAcesso;
        
        if (mesmaArea)
        {
            var tempoAntipassback = Area.TempoAntipassbackTimeSpan.Value;
            dentroDoPassback = timestamp - ultimoAcesso < tempoAntipassback;
            if (dentroDoPassback)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> handleVeiculo(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp)
    {
        var accessHandler = _accessHandlerFactory.GetAccessHandler(alphadigi.Area);

        (bool shouldReturn, string acesso) = await accessHandler.HandleAccessAsync(veiculo, alphadigi);

        // Atualiza informações do veículo (se não for visitante)
        if (veiculo.Id != null)
        {
            await sendUpdateLastAccess(acesso, veiculo.Id, timestamp);
            _logger.LogInformation($"Veículo {veiculo.Placa} atualizado no banco de dados.");

        }

        await sendMonitorAcessoLinear(veiculo, alphadigi.Ip, acesso, timestamp);

        return true; //Sempre retorna true agora
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


    private async Task<bool> sendUpdateLastAccess(string ipCamera,int idVeiculo,DateTime timestamp)
    {
        var lastAccess = new LastAcessUpdateVeiculoDTO
        {
            IdVeiculo = idVeiculo,
            IpCamera = ipCamera,
            TimeAccess = timestamp
        };
        return await _veiculoService.UpdateLastAccess(lastAccess);
    }

    /*
    bool shouldReturn,isVisita,isSaidaSempreAbre,isControlaVaga,isVisitante,abre;
    string acesso;

    var area = alphadigi.Area;
    isVisitante = veiculo.Id==null;
    isVisita = (area.EntradaVisita || area.SaidaVisita) && isVisitante;
    isSaidaSempreAbre = area.SaidaSempreAbre && !alphadigi.Sentido;
    isControlaVaga = area.ControlaVaga;

    if (isVisita)
    {
        acesso = "NÃO CADASTRADO";
        abre = false;
    }
    else if (isSaidaSempreAbre)
    {
        abre = true;
        if (!isVisitante)
        {
            await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
            acesso = "CADASTRADO";
        }
        else
        {
            acesso= "NÃO CADASTRADO";
            abre = true;
        }
    }
    else
    {
        if (isControlaVaga)
        {
            if (!alphadigi.Sentido)
            {
                await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                acesso = "";
                abre = true;
            }
            else
            {
                var vagas = await _unidadeService.GetUnidadeInfo(veiculo.UnidadeNavigation.Id);
                if (vagas.NumVagas > vagas.VagasOcupadasMoradores||veiculo.VeiculoDentro)
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                    acesso = "";
                    abre = true;
                }
                else
                {
                    acesso = "S/VG";
                    abre = false;
                }
            }
        }
        else
        {
            if (!isControlaVaga)
            {
                if (!alphadigi.Sentido)
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                    acesso = "";
                    abre = true;
                }
                else
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                    acesso = "";
                    abre = true;
                }
        }

    }


    }

    if (!isVisita)
    {
        veiculo.DataHoraUltAcesso = timestamp;
        veiculo.IpCamUltAcesso = alphadigi.Ip;
        await _contextSqlite.SaveChangesAsync();
    }

    //_contextSqlite.Veiculo.Update(veiculo);

    return true;
}
    */

    public async Task<Object> handleReturn(string placa, string acesso, bool liberado)
    {
        string info = liberado ? "ok" : "no";
        var retorno = new ResponsePlateDTO
        {
            Response_AlarmInfoPlate = new ResponseAlarmInfoPlate
            {
               info = info,
               content = "retransfer_stop",
            }
        };

        return retorno;
    }
        

}
