﻿using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;
using System.Text.Json;
using Alphadigi_migration.Interfaces;

namespace Alphadigi_migration.Services;

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

    public async Task<object> HandleAlphadigiStage(Alphadigi alphadigi)
    {
        string stage = alphadigi.Estado ?? "DELETE";
        string newStage = null;
        object response = null;
        bool Enviado = false;

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
                    Enviado = false;
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
                    Enviado = false;
                }
                break;
            case "SEND":
                response = await HandleCreate(alphadigi);
                if (response == null)
                {
                    newStage = "FINAL";
                }
                if (Enviado)
                {
                    newStage = response == null ? "FINAL" : "SEND";
                    Enviado = false;
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
            alphadigi.Estado = newStage;
        }

        alphadigi.Enviado = Enviado;
        await _alphadigiService.Update(alphadigi);
        return response;
    }

    public DeleteWhiteListAllDTO HandleDelete(Alphadigi alphadigi)
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
            alphadigi.Enviado = true;
            await _alphadigiService.Update(alphadigi);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em hendleDeleteReturn");
            return false; // Retorna false em caso de erro
        }
    }

    public async Task<AddWhiteListDTO> HandleCreate(Alphadigi alphadigi)
    {
        int ultimoId = alphadigi.UltimoId ?? 0;
        var veiculosEnvio = await _veiculoService.GetVeiculosSend(ultimoId);

        if (veiculosEnvio.Count == 0)
        {
            return null;
        }

        // Atualiza o UltimoId com o ID do último veículo enviado
        alphadigi.UltimoId = veiculosEnvio.Max(item => item.Id);

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
            alphadigi.Enviado = true;
            await _alphadigiService.Update(alphadigi);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro em hendleCreateReturn");
            return false;
        }
    }

    public async Task<ResponseHeathbeatDTO> HandleNormal(Alphadigi alphadigi)
    {
        var condominio = await _condominioService.get();
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

    public async Task<List<SerialData>> sendDisplay(string Nome, Alphadigi alphadigi)
    {
        string? linha1 = "BEM VINDO";
        if (!alphadigi.Sentido)
            linha1 = "ATE LOGO";
        var pacote = await _displayService.RecieveMessageAlphadigi(linha1, Nome, alphadigi);
        return pacote;
    }

}
