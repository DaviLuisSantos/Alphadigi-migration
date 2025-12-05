using Alphadigi_migration.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Alphadigi_migration.Domain.DTOs.Alphadigi;


namespace Alphadigi_migration.Application.Service;

public class AlphadigiHearthBeatService : IAlphadigiHearthBeatService
{
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;
    private readonly ILogger<AlphadigiHearthBeatService> _logger;
    private readonly DisplayService _displayService;
    private readonly CondominioService _condominioService;

    public AlphadigiHearthBeatService(
        IAlphadigiService alphadigiService,
        IVeiculoService veiculoService,
        ILogger<AlphadigiHearthBeatService> logger,
        DisplayService displayService,
        CondominioService condominioService)
    {
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
        _logger = logger;
        _displayService = displayService;
        _condominioService = condominioService;
    }

    public async Task<object> ProcessHearthBeat(string ip)
    {
        _logger.LogInformation($"ProcessHearthBeat chamado com IP: {ip}"); // Log do parâmetro
        try
        {
            var alphadigi = await _alphadigiService.GetOrCreate(ip);
            var hearthBeat = await HandleAlphadigiStage(alphadigi);

            return hearthBeat;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em ProcessHearthBeat");
            throw; // Relance a exceção para que ela seja tratada em um nível superior
        }
    }

    public async Task<object> HandleAlphadigiStage(Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
    {
        string stage = alphadigi.Estado ?? "DELETE";
        string newStage = null;
        object response = null;
        bool enviado = false;

        switch (stage)
        {
            case "DELETE":
                if (!alphadigi.Enviado)
                {
                    response = HandleDelete(alphadigi);
                }
                else
                {
                    newStage = "CREATE";
                    enviado = false;
                }
                break;
            case "CREATE":
                if (!alphadigi.Enviado)
                {
                    response = await HandleCreate(alphadigi);
                    newStage = "SEND";
                }
                else
                {
                    newStage = "SEND";
                    enviado = false;
                }
                break;
            case "SEND":
                response = await HandleCreate(alphadigi);
                if (response == null)
                {
                    newStage = "FINAL";
                }
                if (enviado)
                {
                    newStage = response == null ? "FINAL" : "SEND";
                    enviado = false;
                }
                break;
            case "FINAL":
                response = await HandleNormal(alphadigi);
                newStage = "FINAL";
                break;
            default:
                response = await HandleNormal(alphadigi);
                newStage = "DELETE";
                break;
        }

        if (newStage != null)
        {
            alphadigi.AtualizarEstado(newStage);
        }
        if(enviado)
            alphadigi.MarcarComoEnviado();
        else 
            alphadigi.MarcarComoNaoEnviado();

        await _alphadigiService.Update(alphadigi);
        return response;
    }

    public DeleteWhiteListAllDTO HandleDelete(Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
    {
        return new DeleteWhiteListAllDTO
        {
            DeleteWhiteListAll = 1
        };
    }

    public async Task<bool> HandleDeleteReturn(string ip)
    {
        _logger.LogInformation($"hendleDeleteReturn chamado com IP: {ip}");
        try
        {
            var alphadigi = await _alphadigiService.GetOrCreate(ip);
            alphadigi.MarcarComoEnviado();
            await _alphadigiService.Update(alphadigi);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em hendleDeleteReturn");
            return false; // Retorna false em caso de erro
        }
    }

    public async Task<AddWhiteListDTO> HandleCreate(Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
    {
        int ultimoId = alphadigi.UltimoId ?? 0;
        var veiculosEnvio = await _veiculoService.GetVeiculosSend(ultimoId);

        if (veiculosEnvio.Count == 0)
        {
            return null;
        }

        int novoUltimoId = veiculosEnvio.Max(item => item.Id);
        alphadigi.AtualizarUltimoId(novoUltimoId);

        var envio = new AddWhiteListDTO
        {
            AddWhiteList = new AddWhiteList
            {
                Add_data = veiculosEnvio.Select(item => new AddData
                {
                    Carnum = item.Placa,
                    Startime = "20200718155220",
                    Endtime = "30990718155220"
                }).ToList()
            }
        };

        await _alphadigiService.Update(alphadigi);

        var filePath = "responseCreateHb.json";

        var jsonResult = JsonSerializer.Serialize(envio);

        await File.WriteAllTextAsync(filePath, jsonResult);

        return envio;
    }

    public async Task<bool> HandleCreateReturn(string ip)
    {
        _logger.LogInformation($"hendleCreateReturn chamado com IP: {ip}"); // Log do parâmetro
        try
        {
            var alphadigi = await _alphadigiService.GetOrCreate(ip);
            alphadigi.MarcarComoEnviado();
            await _alphadigiService.Update(alphadigi);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em hendleCreateReturn");
            return false;
        }
    }

    public async Task<ResponseHeathbeatDTO> HandleNormal(Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
    {
        var condominio = await _condominioService.Get();
        var nome = condominio.Nome;
        var messageData = await sendDisplay(nome, alphadigi);
        ResponseHeathbeatDTO retorno = new()
        {
            Response_Heartbeat = new Response_AlarmInfoPlate
            {
                info = "no",
                serialData = messageData
            }
        };

        _logger.LogInformation("🫀 Heartbeat gerado: info={Info}, pacotes={Count}",
        retorno.Response_Heartbeat.info,
        messageData?.Count ?? 0);
        return retorno;
    }

    public async Task<List<SerialData>> sendDisplay(string Nome, Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
    {
        string? linha1 = "BEM VINDO";
        if (!alphadigi.Sentido)
            linha1 = "ATE LOGO";
       var pacote = await _displayService.RecieveMessageAlphadigi(linha1, Nome, alphadigi);
        return pacote ?? new List<SerialData>();
    }

  
}
