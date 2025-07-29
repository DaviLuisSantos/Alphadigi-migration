using Alphadigi_migration.DTO.Display;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;
using System.Drawing;
using Alphadigi_migration.Data;
using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Repositories;
using AutoMapper;

namespace Alphadigi_migration.Services;

public class DisplayService
{
    private readonly MensagemDisplayService _mensagemDisplayService;
    private readonly IMapper _mapper;
    private readonly ILogger<DisplayService> _logger;

    private const string Green = "green";
    private const string Yellow = "yellow";
    private const string Red = "red";
    private const string WelcomeMessage = "BEM VINDO";
    private const string Cadastrado = "CADASTRADO";
    private const string NaoCadastrado = "NÃO CADASTRADO";
    private const string NaoCadastradoFixed = "NAO CADADASTRADO";
    private const string SemVaga = "S/VG";
    private const string SemVagaFixed = "SEM VAGA";
    private const string Liberado = "LIBERADO";
    private const string SinalSerialData = "AGT//w8GAAEAAAAFJLc=";
    private const int SinalSerialDataLen = 14;

    public DisplayService(MensagemDisplayService mensagemDisplayService, IMapper mapper, ILogger<DisplayService> logger)
    {
        _mensagemDisplayService = mensagemDisplayService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<SerialData>> RecieveMessageAlphadigi(string placa, string acesso, Alphadigi alphadigi)
    {
        var serialDataList = new List<SerialData>();
        var packageDisplayList = await PrepareCreatePackage(placa, acesso, alphadigi);

        if (packageDisplayList != null)
        {
            var package = PrepareMessage(packageDisplayList);
            serialDataList.Add(CreateSerialData(package));
        }

        if (acesso == Cadastrado)
        {
            serialDataList.Add(new SerialData
            {
                serialChannel = 0,
                data = SinalSerialData,
                dataLen = SinalSerialDataLen
            });
        }

        return serialDataList;
    }

    public async Task<List<SerialData>> RecieveMessageHearthbeatAlphadigi(string placa, string acesso, Alphadigi alphadigi)
    {
        var serialDataList = new List<SerialData>();
        var packageDisplayList = await PrepareCreatePackage(placa, acesso, alphadigi);

        if (packageDisplayList != null)
        {
            var package = PrepareMessage(packageDisplayList);
            serialDataList.Add(CreateSerialData(package));
        }

        var syncPackage = SyncDateDisplay();
        serialDataList.Add(CreateSerialData(syncPackage));

        return serialDataList;
    }

    public ReturnDataDisplayDTO PrepareMessage(List<CreatePackageDisplayDTO> packageDisplayList)
    {
        var package = Display.CreateMultiLinePackage(packageDisplayList);
        LogPackage(package);
        return new ReturnDataDisplayDTO
        {
            Message = Convert.ToBase64String(package),
            Size = package.Length
        };
    }

    public ReturnDataDisplayDTO SyncDateDisplay()
    {
        var package = Display.CreateTimeSyncPackage();
        LogPackage(package);
        return new ReturnDataDisplayDTO
        {
            Message = Convert.ToBase64String(package),
            Size = package.Length
        };
    }

    public async Task<List<CreatePackageDisplayDTO>> PrepareCreatePackage(string placa, string acesso, Alphadigi alphadigi)
    {
        if (alphadigi.LinhasDisplay == 0 && placa == WelcomeMessage)
            return null;
        var (displayColor, displayAcesso) = GetDisplayColorAndAcesso(placa, acesso);
        var serialDataList = new List<CreatePackageDisplayDTO>
        {
            CreatePlacaDisplayDTO(placa)
        };

        var lastMessage = await _mensagemDisplayService.FindLastMensagem(new FindLastMessage(placa, displayAcesso, alphadigi.Id));
        var lastCamMessage = await _mensagemDisplayService.FindLastCamMensagem(alphadigi.Id);



        // Only save the message if needed
        if (ShouldAddAcessoLine(lastMessage, lastCamMessage, placa))
        {
            // Always add the access/status line
            serialDataList.Add(CreateAcessoDisplayDTO(displayAcesso, displayColor));
            await SaveMensagemDisplayAsync(placa, displayAcesso, alphadigi.Id);
        }
        else if (lastCamMessage?.Placa == WelcomeMessage)
        {
            return null;
        }

        if (alphadigi.LinhasDisplay == 4)
        {
            serialDataList.Add(CreateDateDisplayDTO());
            serialDataList.Add(CreateTimeDisplayDTO());
        }

        return serialDataList;
    }

    #region Private Helper Methods

    private static SerialData CreateSerialData(ReturnDataDisplayDTO package)
    {
        return new SerialData
        {
            serialChannel = 0,
            data = package.Message,
            dataLen = package.Size
        };
    }

    private void LogPackage(byte[] package)
    {
        var hexString = BitConverter.ToString(package).Replace("-", "");
        _logger.LogInformation($"PACOTE GERADO: {hexString}");
    }

    private static (string Color, string Acesso) GetDisplayColorAndAcesso(string placa, string acesso)
    {
        return acesso switch
        {
            "" or Cadastrado => (Green, Liberado),
            NaoCadastrado => (Red, NaoCadastradoFixed),
            SemVaga => (Yellow, SemVagaFixed),
            _ => (placa == WelcomeMessage ? Yellow : Red, acesso == "" ? Liberado : acesso)
        };
    }

    private static CreatePackageDisplayDTO CreatePlacaDisplayDTO(string placa)
    {
        int tempo = placa.Length > 8 ? 1 : 10;
        int estilo = placa.Length > 8 ? 15 : 0;
        return new CreatePackageDisplayDTO
        {
            Mensagem = placa,
            Linha = 1,
            Cor = placa == WelcomeMessage ? Green : Yellow,
            Tempo = tempo,
            Estilo = estilo
        };
    }

    private static CreatePackageDisplayDTO CreateAcessoDisplayDTO(string acesso, string cor)
    {
        int tempo = acesso.Length > 8 ? 1 : 10;
        int estilo = acesso.Length > 8 ? 15 : 0;
        return new CreatePackageDisplayDTO
        {
            Mensagem = acesso,
            Linha = 2,
            Cor = cor,
            Tempo = tempo,
            Estilo = estilo
        };
    }

    private static CreatePackageDisplayDTO CreateDateDisplayDTO()
    {
        return new CreatePackageDisplayDTO
        {
            Mensagem = "`D-`M-`Y",
            Linha = 3,
            Cor = Red,
            Tempo = 0,
            Estilo = 0
        };
    }

    private static CreatePackageDisplayDTO CreateTimeDisplayDTO()
    {
        return new CreatePackageDisplayDTO
        {
            Mensagem = "`H:`N:`S",
            Linha = 4,
            Cor = Yellow,
            Tempo = 0,
            Estilo = 0
        };
    }

    private static bool ShouldAddAcessoLine(MensagemDisplay lastMessage, MensagemDisplay lastCamMessage, string placa)
    {
        return lastMessage == null ||
               (lastCamMessage.Id == 0 || (lastCamMessage != null && lastCamMessage.Id != lastMessage.Id && lastCamMessage.Placa != placa));
    }

    private async Task SaveMensagemDisplayAsync(string placa, string acesso, int alphadigiId)
    {
        var mensagem = new MensagemDisplay
        {
            Placa = placa,
            Mensagem = acesso,
            DataHora = DateTime.Now,
            AlphadigiId = alphadigiId
        };
        await _mensagemDisplayService.SaveMensagemDisplay(mensagem);
    }

    #endregion
}
