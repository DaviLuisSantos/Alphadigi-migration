//using Alphadigi_migration.Domain.DTOs.Alphadigi;
//using Alphadigi_migration.Domain.EntitiesNew;
//using Alphadigi_migration.Domain.Interfaces;
//using Microsoft.Extensions.Logging;

//namespace Alphadigi_migration.Application.Service;

//public interface IAlphadigiPlateService
//{
//    Task<Object> ProcessPlate(ProcessPlateDTO plateReaded);
//}

//public class AlphadigiPlateService : IAlphadigiPlateService
//{
//    private readonly IAlphadigiService _alphadigiService;
//    private readonly IVeiculoService _veiculoService;
//    private readonly ILogger<AlphadigiHearthBeatService> _logger;
//    private readonly AcessoService _acessoService;
//    private readonly IVeiculoAccessProcessor _veiculoAccessProcessor;
//    private readonly IPlacaLidaService _placaLidaService;
//    private readonly DisplayService _displayService;


//    public AlphadigiPlateService(
//            IAlphadigiService alphadigiService,
//            IVeiculoService veiculoService,
//            AcessoService acessoService,
//            ILogger<AlphadigiHearthBeatService> logger,
//            IVeiculoAccessProcessor veiculoAccessProcessor,
//            IPlacaLidaService placaLidaService,
//            DisplayService displayService)
//    {
//        _alphadigiService = alphadigiService;
//        _veiculoService = veiculoService;
//        _logger = logger;
//        _veiculoAccessProcessor = veiculoAccessProcessor;
//        _acessoService = acessoService;
//        _placaLidaService = placaLidaService;
//        _displayService = displayService;
//    }

//    public async Task<object> ProcessPlate(ProcessPlateDTO plateReaded)
//    {
//        try
//        {
//            DateTime timeStamp = DateTime.Now;

//            _logger.LogInformation("placa recebida: " + plateReaded.plate + " Camera: " + plateReaded.ip);

//            var camera = await _alphadigiService.GetOrCreate(plateReaded.ip);
//            if (camera == null)
//            {
//                _logger.LogError($"Câmera não encontrada para o IP {plateReaded.ip}.");
//                throw new Exception("Camera não encontrada");
//            }
//            if (camera.LinhasDisplay != 0 && plateReaded.modelo == "TOTEM")
//            {
                
//                camera.AtualizarLinhasDisplay(0);
//                camera = await _alphadigiService.Update(camera);
//            }

//            var Log = new PlacaLida
//            (
//                alphadigiId: camera.Id,
//                placa: plateReaded.plate,
//                dataHora: timeStamp,
//                areaId: camera.AreaId,
//                placaImg: plateReaded.plateImage,
//                carroImg: plateReaded.carImage

//            );

//            await _placaLidaService.SavePlacaLida(Log);

//            bool veiculoCadastrado = true;
//            var veiculo = await _veiculoService.GetByPlate(plateReaded.plate);
//            if (veiculo == null)
//            {
//                veiculoCadastrado = false;
//                veiculo = new Veiculo
//                (
//                    placa: plateReaded.plate
                    

//                );
//            }
//            if (veiculo != null && !plateReaded.isCad && veiculoCadastrado)
//            {
//                //07/02/2025 - Retirado estava entrando em envio toda hora, atrapalhando a mensagem stand by do display
//                //camera.UltimoId = veiculo.Id - 1;
//                //camera.Estado = "SEND";
//                camera.AtualizarUltimoId(Guid.Empty);
//                await _alphadigiService.Update(camera);
//            }

//           // Log.Cadastrado = veiculoCadastrado;
//            Log.AtualizarCadastro(veiculoCadastrado);
//            await _placaLidaService.UpdatePlacaLida(Log);

//            var accessResult = await sendVeiculoAccessProcessor(veiculo, camera, timeStamp, Log, plateReaded.carImage);
//            _logger.LogInformation($"Accesso do veículo com a placa {plateReaded.plate} com resultado {accessResult}");

//            //Log.Liberado = accessResult.ShouldReturn;

//            // Log.Acesso = accessResult.Acesso;
//            Log.MarcarComoProcessado(accessResult.ShouldReturn, accessResult.Acesso);

//            await _placaLidaService.UpdatePlacaLida(Log);

//            var messageDisplay = await sendCreatPackageDisplay(veiculo, accessResult.Acesso, camera);

//            if (Log.Processado)
//            {
//                return await handleReturn(veiculo.Placa, accessResult.Acesso, accessResult.ShouldReturn, messageDisplay);
//            }

//            return await ProcessPlate(plateReaded);

//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, $"Erro em ProcessPlate.");
//            throw;
//        }
//    }

//    public async Task<bool> VerifyAntiPassback(Veiculo veiculo, Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi, DateTime timestamp)
//    {
//        Area? Area, ultimaArea;
//        DateTime? ultimoAcesso;
//        bool mesmaCamera, mesmaArea, dentroDoPassback;
//        mesmaCamera = veiculo.IpCamUltAcesso == alphadigi.Ip;
//        Area = alphadigi.Area;

//        if (mesmaCamera)
//        {
//            mesmaArea = true;
//        }
//        else
//        {
//            var ultimaCamera = await _alphadigiService.GetOrCreate(veiculo.IpCamUltAcesso);
//            ultimaArea = ultimaCamera.Area;
//            mesmaArea = ultimaCamera.AreaId == alphadigi.AreaId;
//        }

//        ultimoAcesso = veiculo.DataHoraUltAcesso;

//        if (mesmaArea)
//        {
//           // var tempoAntipassback = Area.TempoAntipassbackTimeSpan.Value;
//            TimeSpan? tempoAntipassback = alphadigi.Area.TempoAntipassback;
//            dentroDoPassback = timestamp - ultimoAcesso < tempoAntipassback.Value;

//            if (dentroDoPassback)
//            {
//                return false;
//            }
//        }

//        return true;
//    }

//    public async Task<(bool ShouldReturn, string Acesso)> sendVeiculoAccessProcessor(Veiculo veiculo, Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi, DateTime timestamp, PlacaLida log, string? imagem)
//    {
//        (bool shouldReturn, string acesso) = await _veiculoAccessProcessor.ProcessVeiculoAccessAsync(veiculo, alphadigi, timestamp);

//        if (veiculo.Placa.Numero != null)
//        {
//            await _alphadigiService.UpdateLastPlate(alphadigi, veiculo.Placa, timestamp);
//            await _acessoService.SaveVeiculoAcesso(alphadigi, veiculo, timestamp, imagem);
//            _logger.LogInformation($"Veículo {veiculo.Placa} atualizado no banco de dados.");
//        }

//        //log.Processado = true;
//        log.MarcarComoProcessado(shouldReturn, acesso);
     
//        await _placaLidaService.UpdatePlacaLida(log);
//        return (shouldReturn, acesso);
//    }

//    public async Task<List<SerialData>> sendCreatPackageDisplay(Veiculo veiculo, string acesso, Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
//    {
//        string placaFormatada = veiculo.Placa;
//        if (!string.IsNullOrEmpty(placaFormatada) && placaFormatada.Length > 3)
//        {
//            placaFormatada = placaFormatada.Insert(3, "-");
//        }
//        return await _displayService.RecieveMessageAlphadigi(placaFormatada, acesso, alphadigi);
//    }

//    public async Task<ResponsePlateDTO> handleReturn(string placa, string acesso, bool liberado, List<SerialData> messageData)
//    {
//        string info = liberado ? "ok" : "no";
//        var retorno = new ResponsePlateDTO
//        {
//            Response_AlarmInfoPlate = new ResponseAlarmInfoPlate
//            {
//                info = info,
//                serialData = messageData
//            }
//        };

//        return retorno;
//    }


//}
