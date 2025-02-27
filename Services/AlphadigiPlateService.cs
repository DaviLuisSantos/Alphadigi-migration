using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.DTO.Veiculo;
using Alphadigi_migration.DTO.Display;

namespace Alphadigi_migration.Services;

public interface IAlphadigiPlateService
{
    Task<Object> ProcessPlate(ProcessPlateDTO plateReaded);
}

public class AlphadigiPlateService : IAlphadigiPlateService
{
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;
    private readonly ILogger<AlphadigiHearthBeatService> _logger;
    private readonly AcessoService _acessoService;
    private readonly IVeiculoAccessProcessor _veiculoAccessProcessor;


    public AlphadigiPlateService(
            IAlphadigiService alphadigiService,
            IVeiculoService veiculoService,
            AcessoService acessoService,
            ILogger<AlphadigiHearthBeatService> logger,
            IVeiculoAccessProcessor veiculoAccessProcessor) // Adicione o logger
    {
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
        _logger = logger; // Salve o logger
        _veiculoAccessProcessor = veiculoAccessProcessor;
        _acessoService = acessoService;
    }

    public async Task<object> ProcessPlate(ProcessPlateDTO plateReaded)
    {
        _logger.LogInformation($"Iniciando ProcessPlate");
        try
        {
            DateTime timeStamp = DateTime.Now;

            var camera = await _alphadigiService.GetOrCreate(plateReaded.ip);
            if (camera == null)
            {
                _logger.LogError($"Câmera não encontrada para o IP {plateReaded.ip}.");
                throw new Exception("Camera não encontrada");
            }

            var veiculo = await _veiculoService.getByPlate(plateReaded.plate);
            if (veiculo == null)
            {
                veiculo = new Veiculo
                {
                    Placa = plateReaded.plate,
                };
            }
            _logger.LogInformation($"Veículo encontrado para a placa {plateReaded.plate}.");

            //Remove a chamada para AntiPassBack caso necessário, e já retorna para o ponto centralizado

            var accessResult = await sendVeiculoAccessProcessor(veiculo, camera, timeStamp);
            _logger.LogInformation($"Accesso do veículo com a placa {plateReaded.plate} com resultado {accessResult}");

            var messageDisplay = sendCreatPackageDisplay(veiculo, accessResult.Acesso);

            return await handleReturn(veiculo.Placa, accessResult.Acesso, accessResult.ShouldReturn, messageDisplay);

        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro em ProcessPlate.");
            throw; //Sempre relance a exceção caso não consiga resolver!
        }
    }

    public async Task<bool> VerifyAntiPassback(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp)
    {
        Area? Area, ultimaArea;
        DateTime? ultimoAcesso;
        bool mesmaCamera, mesmaArea, dentroDoPassback;
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

    public async Task<(bool ShouldReturn, string Acesso)> sendVeiculoAccessProcessor(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp)
    {
        (bool shouldReturn, string acesso) = await _veiculoAccessProcessor.ProcessVeiculoAccessAsync(veiculo, alphadigi, timestamp);

        // Atualiza informações do veículo (se não for visitante)
        if (veiculo.Placa != null)
        {
            await _alphadigiService.UpdateLastPlate(alphadigi, veiculo.Placa, timestamp);
            await _acessoService.saveVeiculoAcesso(alphadigi, veiculo, timestamp);
            _logger.LogInformation($"Veículo {veiculo.Placa} atualizado no banco de dados.");
        }

        return (shouldReturn, acesso);
    }

    public List<SerialData> sendCreatPackageDisplay(Veiculo veiculo, string acesso)
    {
        return DisplayService.recieveMessageAlphadigi(veiculo, acesso);
    }

    public async Task<ResponsePlateDTO> handleReturn(string placa, string acesso, bool liberado, List<SerialData> messageData)
    {
        string info = liberado ? "ok" : "no";
        var retorno = new ResponsePlateDTO
        {
            Response_AlarmInfoPlate = new ResponseAlarmInfoPlate
            {
                info = info,
                content = "retransfer_stop",
                serialData = messageData
            }
        };

        return retorno;
    }


}
