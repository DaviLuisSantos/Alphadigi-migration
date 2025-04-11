using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Services;

public interface IAlphadigiHearthBeatService
{
    Task<object> ProcessHearthBeat(string ip);
    Task<object> HandleAlphadigiStage(Alphadigi alphadigi);
    DeleteWhiteListAllDTO HandleDelete(Alphadigi alphadigi);
    Task<AddWhiteListDTO> HandleCreate(Alphadigi alphadigi);
}

public class AlphadigiHearthBeatService : IAlphadigiHearthBeatService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;
    private readonly ILogger<AlphadigiHearthBeatService> _logger;
    private readonly DisplayService _displayService;
    private readonly CondominioService _condominioService;

    public AlphadigiHearthBeatService(
        AppDbContextSqlite contextSqlite,
        IAlphadigiService alphadigiService,
        IVeiculoService veiculoService,
        ILogger<AlphadigiHearthBeatService> logger,
        DisplayService displayService,
        CondominioService condominioService)
    {
        _contextSqlite = contextSqlite;
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

        switch (stage)
        {
            case "DELETE":
                response = HandleDelete(alphadigi);
                newStage = "CREATE";
                break;
            case "CREATE":
                response = await HandleCreate(alphadigi);
                newStage = "SEND";
                break;
            case "SEND":
                response = await HandleCreate(alphadigi);
                newStage = response == null ? "FINAL" : "SEND";
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

        alphadigi.Estado = newStage;
        _contextSqlite.Update(alphadigi);
        await _contextSqlite.SaveChangesAsync();
        return response;
    }

    public DeleteWhiteListAllDTO HandleDelete(Alphadigi alphadigi)
    {
        return new DeleteWhiteListAllDTO
        {
            DeleteWhiteListAll = 1
        };
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
                add_data = veiculosEnvio.Select(item => new AddData
                {
                    Carnum = item.Placa,
                    Startime = "20200718155220",
                    Endtime = "30990718155220"
                }).ToList()
            }
        };
        // Salva as mudanças no banco de dados
        _contextSqlite.Update(alphadigi);
        await _contextSqlite.SaveChangesAsync();

        return envio;
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
        var linha1 = "BEM VINDO";
        var pacote = await _displayService.recieveMessageHearthbeatAlphadigi(linha1, Nome, alphadigi);
        return pacote;
    }

}
