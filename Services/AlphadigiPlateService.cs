using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Services;

public class AlphadigiPlateService: IAlphadigiPlateService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;
    private readonly ILogger<AlphadigiHearthBeatService> _logger;


    public AlphadigiPlateService(
            AppDbContextSqlite contextSqlite,
            AppDbContextFirebird contextFirebird,
            IAlphadigiService alphadigiService,
            IVeiculoService veiculoService,
            ILogger<AlphadigiHearthBeatService> logger) // Adicione o logger
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
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

        if (veiculo == null)
        {
            return await handleReturn(plateReaded.plate, plateReaded.ip, true);
        }

        if(plateReaded.ip != veiculo.IpCamUltAcesso)
        {

        }
        return veiculo;
    }

    public async Task<Object> handleReturn(string placa, string acesso, bool liberado)
    {
        string info = liberado ? "ok" : "no";
        var retorno = new ResponsePlateDTO
        {
            Response_AlarmInfoPlate = new ResponseAlarmInfoPlate
            {
                Info = info,
                Content = "retransfer_stop",
            }
        };
        return retorno;
    }

}
