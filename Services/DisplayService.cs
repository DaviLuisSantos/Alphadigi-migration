using Alphadigi_migration.DTO.Display;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;
using System.Drawing;
using Alphadigi_migration.Data;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public class DisplayService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly ILogger<DisplayService> _logger;

    public DisplayService(AppDbContextSqlite contextSqlite, ILogger<DisplayService> logger)
    {
        _contextSqlite = contextSqlite;
        _logger = logger;
    }

    public async Task<List<SerialData>> recieveMessageAlphadigi(string placa, string acesso, Alphadigi alphadigi)
    {
        var createPackageDisplayDTO = await prepareCreatePackage(placa, acesso, alphadigi);
        var serialData = new List<SerialData>();

        if (createPackageDisplayDTO != null)
        {
            var package = PrepareMessage(createPackageDisplayDTO);
            var serialDataItem = new SerialData
            {
                serialChannel = 0,
                data = package.Message,
                dataLen = package.Size
            };

            serialData.Add(serialDataItem);
        }
        if (acesso == "CADASTRADO")
        {
            var sinalSerial = new SerialData
            {
                serialChannel = 0,
                data = "AGT//w8GAAEAAAAFJLc=",
                dataLen = 14
            };
            serialData.Add(sinalSerial);
        }
        return serialData;
    }

    public async Task<List<SerialData>> recieveMessageHearthbeatAlphadigi(string placa, string acesso, Alphadigi alphadigi)
    {
        var createPackageDisplayDTO = await prepareCreatePackage(placa, acesso, alphadigi);
        var serialData = new List<SerialData>();

        if (createPackageDisplayDTO != null)
        {
            var package = PrepareMessage(createPackageDisplayDTO);
            var serialDataItem = new SerialData
            {
                serialChannel = 0,
                data = package.Message,
                dataLen = package.Size
            };

            serialData.Add(serialDataItem);
        }
        var dataUpdate = syncDateDisplay();
        var serialDataItem2 = new SerialData
        {
            serialChannel = 0,
            data = dataUpdate.Message,
            dataLen = dataUpdate.Size
        };
        serialData.Add(serialDataItem2);
        return serialData;
    }

    public ReturnDataDisplayDTO PrepareMessage(List<CreatePackageDisplayDTO> createPackageDisplayDTO)
    {
        var package = Display.CreateMultiLinePackage(createPackageDisplayDTO);

        // Convert the byte array to a hexadecimal string
        var hexString = BitConverter.ToString(package).Replace("-", "");
        _logger.LogInformation($"PACOTE GERADO: {hexString}");

        var package64 = Convert.ToBase64String(package);

        var returnDataDisplayDTO = new ReturnDataDisplayDTO
        {
            Message = package64,
            Size = package.Length
        };

        return returnDataDisplayDTO;
    }
    public ReturnDataDisplayDTO syncDateDisplay()
    {
        var package = Display.CreateTimeSyncPackage();
        // Convert the byte array to a hexadecimal string
        var hexString = BitConverter.ToString(package).Replace("-", "");
        _logger.LogInformation($"PACOTE GERADO: {hexString}");
        var package64 = Convert.ToBase64String(package);
        var returnDataDisplayDTO = new ReturnDataDisplayDTO
        {
            Message = package64,
            Size = package.Length
        };
        return returnDataDisplayDTO;
    }

    public async Task<List<CreatePackageDisplayDTO>> prepareCreatePackage(string placa, string acesso, Alphadigi alphadigi)
    {
        string cor = "red";

        switch (acesso)
        {
            case "":
            case "CADASTRADO":
                cor = "green";
                acesso = "LIBERADO";
                break;
            case "NÃO CADASTRADO":
                acesso = "NAO CADADASTRADO";
                break;
            case "S/VG":
                cor = "yellow";
                acesso = "SEM VAGA";
                break;
        }

        if (acesso != "NAO CADADASTRADO" && placa == "BEM VINDO") cor = "yellow";

        acesso = acesso == "" ? "LIBERADO" : acesso;
        int tempo = 10, estilo = 0;

        var serialData = new List<CreatePackageDisplayDTO>();

        var packageDisplayPlaca = new CreatePackageDisplayDTO
        {
            Mensagem = placa,
            Linha = 1,
            Cor = placa == "BEM VINDO" ? "green" : "yellow",
            Tempo = placa.Length > 8 ? 1 : tempo,
            Estilo = placa.Length > 8 ? 15 : estilo
        };
        serialData.Add(packageDisplayPlaca);

        MensagemDisplay Mensagem = new();

        var LastMessage = _contextSqlite.MensagemDisplay
            .Where(x => x.Placa == placa)
            .Where(x => x.Mensagem == acesso)
            .Where(x => x.DataHora.AddSeconds(12) > DateTime.Now)
            .Where(x => x.AlphadigiId == alphadigi.Id)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        var LastCamMessage = _contextSqlite.MensagemDisplay
            .Where(x => x.AlphadigiId == alphadigi.Id)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if ((LastMessage == null || LastCamMessage.Id != LastMessage.Id) && (LastCamMessage.Placa != placa))
        {
            if (acesso.Length > 8)
            {
                estilo = 15;
                tempo = 1;
            }
            var packageDisplayAcesso = new CreatePackageDisplayDTO
            {
                Mensagem = acesso,
                Linha = 2,
                Cor = cor,
                Tempo = tempo,
                Estilo = estilo
            };
            serialData.Add(packageDisplayAcesso);
            MensagemDisplay mensagem = new()
            {
                Placa = placa,
                Mensagem = acesso,
                DataHora = DateTime.Now,
                AlphadigiId = alphadigi.Id,
            };
            await SaveMensagemDisplay(mensagem);
        }
        else if (LastCamMessage.Placa == "BEM VINDO")
        {
            return null;
        }

        var packageDisplayData = new CreatePackageDisplayDTO
        {
            Mensagem = "`D-`M-`Y",
            Linha = 3,
            Cor = "red",
            Tempo = 0,
            Estilo = 0
        };
        serialData.Add(packageDisplayData);

        var packageDisplayHora = new CreatePackageDisplayDTO
        {
            Mensagem = "`H:`N:`S",
            Linha = 4,
            Cor = "yellow",
            Tempo = 0,
            Estilo = 0
        };
        serialData.Add(packageDisplayHora);

        return serialData;
    }

    public async Task<bool> SaveMensagemDisplay(MensagemDisplay mensagem)
    {
        _contextSqlite.MensagemDisplay.Add(mensagem);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }
}
