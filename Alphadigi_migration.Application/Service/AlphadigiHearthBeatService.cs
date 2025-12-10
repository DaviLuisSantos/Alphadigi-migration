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
                _logger.LogInformation("📤 Executando SEND - Enviado: {Enviado}, UltimoId: {UltimoId}",
                    alphadigi.Enviado, alphadigi.UltimoId);

                response = await HandleCreate(alphadigi);

                if (response == null)
                {
                    _logger.LogInformation("✅ Nenhum veículo encontrado - indo para FINAL");
                    newStage = "FINAL";
                    alphadigi.MarcarComoNaoEnviado(); // Reset para próximo ciclo
                    alphadigi.AtualizarUltimoId(null); // Resetar UltimoId
                }
                else
                {
                    _logger.LogInformation("📊 Dados encontrados - mantendo SEND");
                    newStage = "SEND";
                    alphadigi.MarcarComoEnviado(); // Marcar que já enviou
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
        // LÓGICA CORRIGIDA:
        // Se está em SEND e JÁ FOI ENVIADO, buscar APÓS o UltimoId
        // Se está em SEND mas NÃO FOI ENVIADO, buscar DO ZERO (primeira vez)
        // Se está em CREATE, buscar DO ZERO

        int ultimoId;

        if (alphadigi.Estado == "SEND" && alphadigi.Enviado)
        {
            // Já enviou antes - buscar novos após o último ID
            ultimoId = alphadigi.UltimoId ?? 0;
            _logger.LogInformation("📡 Buscando NOVOS veículos após ID: {UltimoId}", ultimoId);
        }
        else
        {
            // Primeira vez no SEND ou está em CREATE - buscar do zero
            ultimoId = 0;
            _logger.LogInformation("🆕 Buscando TODOS os veículos (estado: {Estado})", alphadigi.Estado);
        }

        var veiculosEnvio = await _veiculoService.GetVeiculosSend(ultimoId);

        _logger.LogInformation("📊 Veículos encontrados após ID {UltimoId}: {Count}",
            ultimoId, veiculosEnvio.Count);

        if (veiculosEnvio.Count == 0)
        {
            return null;
        }

        // Atualizar UltimoId com o máximo encontrado
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
            Response_Heartbeat = new ResponseAlarmInfoPlate
            {
                info = "no",
                serialData = messageData
            }
        };
        return retorno;
    }

    public async Task<List<SerialData>> sendDisplay(string Nome, Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi)
    {
        string? linha1 = "BEM VINDO";
        if (!alphadigi.Sentido)
            linha1 = "ATE LOGO";
       // var pacote = await _displayService.RecieveMessageAlphadigi(linha1, Nome, alphadigi);
        return null;
    }

  
}
