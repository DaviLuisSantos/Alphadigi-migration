using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Service;

public class DisplayService
{
    private readonly MensagemDisplayService _mensagemDisplayService;
    private readonly ILogger<DisplayService> _logger;
    private readonly IAlphadigiRepository _alphadigiRepository;

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
    private bool _isDisplayInitialized = false;

    public DisplayService(
        MensagemDisplayService mensagemDisplayService,
        ILogger<DisplayService> logger, IAlphadigiRepository alphadigiRepository)
    {
        _mensagemDisplayService = mensagemDisplayService;
        _logger = logger;
        _alphadigiRepository = alphadigiRepository;
    }

    public async Task InitializeDisplay(Domain.EntitiesNew.Alphadigi alphadigi)
    {
        if (!_isDisplayInitialized)
        {
            _logger.LogInformation("🔄 Inicializando display com data/hora padrão");

            // Envia data/hora como conteúdo padrão
            var defaultPackage = await CreateDefaultDisplayPackage(alphadigi);

            // Marca como inicializado
            _isDisplayInitialized = true;

            _logger.LogInformation("✅ Display inicializado com conteúdo padrão");
        }
    }
    private async Task<List<SerialData>> CreateDefaultDisplayPackage(Domain.EntitiesNew.Alphadigi alphadigi)
    {
        var serialDataList = new List<SerialData>();

        if (alphadigi.LinhasDisplay == 4)
        {
            // Para display de 4 linhas: mostra data e hora permanentemente
            var defaultLines = new List<CreatePackageDisplayDTO>
            {
                new() { Mensagem = "", Linha = 1, Cor = Green, Tempo = 0, Estilo = 0 }, // Linha vazia
                new() { Mensagem = "", Linha = 2, Cor = Green, Tempo = 0, Estilo = 0 }, // Linha vazia
                new() { Mensagem = "`D/`M/`Y", Linha = 3, Cor = Red, Tempo = 0, Estilo = 0 }, // Data
                new() { Mensagem = "`H:`N:`S", Linha = 4, Cor = Yellow, Tempo = 0, Estilo = 0 } // Hora
            };

            var package = PrepareMessage(defaultLines);
            serialDataList.Add(CreateSerialData(package));
        }
        else if (alphadigi.LinhasDisplay == 2)
        {
            // Para display de 2 linhas: mostra "BEM VINDO" permanentemente
            var defaultLines = new List<CreatePackageDisplayDTO>
            {
                new() { Mensagem = "BEM VINDO", Linha = 1, Cor = Green, Tempo = 0, Estilo = 0 },
                new() { Mensagem = "AGUARDANDO", Linha = 2, Cor = Yellow, Tempo = 0, Estilo = 0 }
            };

            var package = PrepareMessage(defaultLines);
            serialDataList.Add(CreateSerialData(package));
        }

        return serialDataList;
    }


    public async Task<List<SerialData>> RecieveMessageAlphadigi(
    string placa, string acesso, Domain.EntitiesNew.Alphadigi alphadigi)
    {
        await InitializeDisplay(alphadigi);

        var serialDataList = new List<SerialData>();

    
      
        var packageDisplayList = await PrepareCreatePackage(placa, acesso, alphadigi);
        _logger.LogInformation($"📦 Resultado GenerateDisplayPackage: {(packageDisplayList != null ? "SUCESSO" : "NULL")}");

        if (packageDisplayList != null)
        {
            var package = PrepareMessage(packageDisplayList);
            serialDataList.Add(CreateSerialData(package));
            _logger.LogInformation("📦 Pacote da placa enviado DEPOIS");
        }
        

        
        if (acesso == Cadastrado)
        {
            serialDataList.Add(new SerialData
            {
                serialChannel = 0,
                data = SinalSerialData,
                dataLen = SinalSerialDataLen
            });
            _logger.LogInformation("✅ Sinal verde enviado para placa cadastrada");
        }

       
        _logger.LogInformation($"✅ Pacote gerado com {serialDataList.Count} linhas");
        return serialDataList;
    }

    public async Task<List<SerialData>> RecieveMessageHearthbeatAlphadigi(
    string placa, string acesso, Domain.EntitiesNew.Alphadigi alphadigi)
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
        _logger.LogInformation($"📦 Criando pacote com {packageDisplayList.Count} linhas");
        foreach (var line in packageDisplayList)
        {
            _logger.LogInformation($"  Linha {line.Linha}: '{line.Mensagem}' (Tempo={line.Tempo}s, Estilo={line.Estilo})");
        }

        var package = Display.CreateMultiLinePackage(packageDisplayList);

        _logger.LogInformation("🔍 ANÁLISE DO PACOTE:");
        _logger.LogInformation($"  Comando: 0x6E (Multi-line display)");
        _logger.LogInformation($"  SaveFlag: 0x{package[6]:X2} (0=RAM, 1=Flash)");
        _logger.LogInformation($"  TextContextNumber: 0x{package[7]:X2} ({package[7]} linhas)");


        int index = 8;
        for (int i = 0; i < packageDisplayList.Count; i++)
        {
            byte lineId = package[index];
            byte dm = package[index + 1];
            byte ds = package[index + 2];
            byte dt = package[index + 3]; // ⚠️ Dwell Time
            byte dr = package[index + 4];

            _logger.LogInformation($"  Linha {i + 1}: LID={lineId}, DM={dm}, DS={ds}, DT={dt}s, DR={dr}");

            // Pular para próxima linha (LID + DM + DS + DT + DR + TC[4] + TL + TEXT)
            int textLength = package[index + 9]; // TL
            index += 10 + textLength + 1; // +1 para o separador
        }
        // ⭐⭐ LOG HEX DETALHADO ⭐⭐
        _logger.LogInformation("🔍🔍🔍 PACOTE HEX COMPLETO (65 bytes):");
        string hex = BitConverter.ToString(package).Replace("-", "");

        // Log em grupos de 32 caracteres (16 bytes)
        for (int i = 0; i < hex.Length; i += 32)
        {
            int length = Math.Min(32, hex.Length - i);
            _logger.LogInformation($"  {i / 2:X4}: {hex.Substring(i, length)}");
        }


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

    public async Task<List<CreatePackageDisplayDTO>> PrepareCreatePackage(
     string placa, string acesso, Domain.EntitiesNew.Alphadigi alphadigi)
    {
        if (alphadigi.LinhasDisplay == 0 && placa == WelcomeMessage)
            return null;

        _logger.LogInformation($"🔍 AAAAA LinhasDisplay: {alphadigi.LinhasDisplay}, Placa: {placa}");

        var (displayColor, displayAcesso) = GetDisplayColorAndAcesso(placa, acesso);
        var serialDataList = new List<CreatePackageDisplayDTO>
        {
            CreatePlacaDisplayDTO(placa)
        };

        var lastMessage = await _mensagemDisplayService.FindLastMensagemAsync(new FindLastMessage(placa, displayAcesso, alphadigi.Id));
        var lastCamMessage = await _mensagemDisplayService.FindLastCamMensagemAsync(alphadigi.Id);



        // Only save the message if needed
        if (ShouldAddAcessoLine(lastMessage, lastCamMessage, placa))
        {
            // Always add the access/status line
            serialDataList.Add(CreateAcessoDisplayDTO(displayAcesso, displayColor));
            await SaveMensagemDisplayAsync(placa, displayAcesso, alphadigi.Id);
        }
        else if (lastCamMessage?.Placa == WelcomeMessage)
        {
            // return null;
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
        _logger.LogDebug($"📦 Pacote display (hex): {hexString}");
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
            Tempo = 10,   
            Estilo = 0
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
            Tempo = 10,
            Estilo = 0
        };
    }

    private static CreatePackageDisplayDTO CreateDateDisplayDTO()
    {
        return new CreatePackageDisplayDTO
        {
            Mensagem = "`D/`M/`Y",  
            Linha = 3,              
            Cor = Red,
            Tempo = 10,              
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
            Tempo = 10,               
            Estilo = 0
        };
    }

    private static bool ShouldAddAcessoLine(Domain.EntitiesNew.MensagemDisplay lastMessage, 
                                            Domain.EntitiesNew.MensagemDisplay lastCamMessage, 
                                            string placa)
    {
        return lastMessage == null ||
               (lastCamMessage.Id == 0 || (lastCamMessage != null && lastCamMessage.Id != lastMessage.Id && lastCamMessage.Placa != placa));
    }

    private async Task SaveMensagemDisplayAsync(string placa, string acesso, int alphadigiId)
    {
        // Se for "BEM VINDO", não salve como placa
        if (placa == WelcomeMessage)
        {
            _logger.LogInformation("📝 Ignorando salvamento de mensagem para 'BEM VINDO'");
            return;
        }

        // Valida se é uma placa válida (máximo 8 caracteres)
        if (placa.Length > 8)
        {
            _logger.LogWarning($"⚠️ Placa muito longa para salvar: '{placa}' ({placa.Length} caracteres)");
            return;
        }

        var mensagem = new Domain.EntitiesNew.MensagemDisplay
        (
            placa: placa,
            mensagem: acesso,
            dataHora: DateTime.Now,
            alphadigiId: alphadigiId
        );
        await _mensagemDisplayService.SaveMensagemDisplayAsync(mensagem);
    }


    #endregion
}