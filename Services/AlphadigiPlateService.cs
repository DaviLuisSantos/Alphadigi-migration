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
    private readonly PlacaLidaService _placaLidaService;
    private readonly DisplayService _displayService;


    public AlphadigiPlateService(
            IAlphadigiService alphadigiService,
            IVeiculoService veiculoService,
            AcessoService acessoService,
            ILogger<AlphadigiHearthBeatService> logger,
            IVeiculoAccessProcessor veiculoAccessProcessor,
            PlacaLidaService placaLidaService,
            DisplayService displayService) // Adicione o logger
    {
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
        _logger = logger; // Salve o logger
        _veiculoAccessProcessor = veiculoAccessProcessor;
        _acessoService = acessoService;
        _placaLidaService = placaLidaService;
        _displayService = displayService;
    }

    public async Task<object> ProcessPlate(ProcessPlateDTO plateReaded)
    {
        try
        {
            DateTime timeStamp = DateTime.Now;

            _logger.LogInformation("placa recebida: " + plateReaded.plate);

            var camera = await _alphadigiService.GetOrCreate(plateReaded.ip);
            if (camera == null)
            {
                _logger.LogError($"Câmera não encontrada para o IP {plateReaded.ip}.");
                throw new Exception("Camera não encontrada");
            }

            var Log = new PlacaLida
            {
                AlphadigiId = camera.Id,
                Placa = plateReaded.plate,
                DataHora = timeStamp,
                AreaId = camera.AreaId,
                Placa_Img = plateReaded.plateImage,
                Carro_Img = plateReaded.carImage

            };

            await _placaLidaService.SavePlacaLida(Log);

            bool veiculoCadastrado = true;
            var veiculo = await _veiculoService.getByPlate(plateReaded.plate);
            if (veiculo == null)
            {
                veiculoCadastrado = false;
                veiculo = new Veiculo
                {
                    Placa = plateReaded.plate,
                };
            }
            Log.Cadastrado = veiculoCadastrado;
            await _placaLidaService.UpdatePlacaLida(Log);

            var accessResult = await sendVeiculoAccessProcessor(veiculo, camera, timeStamp, Log);
            _logger.LogInformation($"Accesso do veículo com a placa {plateReaded.plate} com resultado {accessResult}");

            Log.Liberado = accessResult.ShouldReturn;
            Log.Acesso = accessResult.Acesso;

            await _placaLidaService.UpdatePlacaLida(Log);

            var messageDisplay = await sendCreatPackageDisplay(veiculo, accessResult.Acesso, camera);

            if (Log.Processado)
            {
                return await handleReturn(veiculo.Placa, accessResult.Acesso, accessResult.ShouldReturn, messageDisplay);
            }

            return await ProcessPlate(plateReaded);

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

    public async Task<(bool ShouldReturn, string Acesso)> sendVeiculoAccessProcessor(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp, PlacaLida log)
    {
        (bool shouldReturn, string acesso) = await _veiculoAccessProcessor.ProcessVeiculoAccessAsync(veiculo, alphadigi, timestamp);

        if (veiculo.Placa != null)
        {
            await _alphadigiService.UpdateLastPlate(alphadigi, veiculo.Placa, timestamp);
            await _acessoService.saveVeiculoAcesso(alphadigi, veiculo, timestamp);
            _logger.LogInformation($"Veículo {veiculo.Placa} atualizado no banco de dados.");
        }

        log.Processado = true;
        await _placaLidaService.UpdatePlacaLida(log);
        return (shouldReturn, acesso);
    }

    public async Task<List<SerialData>> sendCreatPackageDisplay(Veiculo veiculo, string acesso, Alphadigi alphadigi)
    {
        return await _displayService.recieveMessageAlphadigi(veiculo.Placa, acesso, alphadigi);
    }

    public async Task<ResponsePlateDTO> handleReturn(string placa, string acesso, bool liberado, List<SerialData> messageData)
    {
        string info = liberado ? "ok" : "no";
        var retorno = new ResponsePlateDTO
        {
            Response_AlarmInfoPlate = new ResponseAlarmInfoPlate
            {
                info = info,
                serialData = messageData
            }
        };

        return retorno;
    }


}
