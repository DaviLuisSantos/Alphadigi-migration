using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Domain.EntitiesNew;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Service;

public class DisplayService
{
    private readonly MensagemDisplayService _mensagemDisplayService;
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

    public DisplayService(
        MensagemDisplayService mensagemDisplayService,
        ILogger<DisplayService> logger)
    {
        _mensagemDisplayService = mensagemDisplayService;
        _logger = logger;
    }

    public async Task<List<SerialData>> RecieveMessageAlphadigi(
     string placa,
     string acesso,
     Domain.EntitiesNew.Alphadigi alphadigi)
    {
        var serialDataList = new List<SerialData>();

        _logger.LogInformation("🔄 ORDENANDO PACOTES para AlphaDigi");

        // 🔄 TENTE ESTA ORDEM DIFERENTE:
        // 1. PRIMEIRO o sinal serial (se for CADASTRADO)
        if (acesso.ToUpper() == "CADASTRADO")
        {
            serialDataList.Add(new SerialData
            {
                serialChannel = 0,
                data = "AGT//w8GAAEAAAAFJLc=",
                dataLen = 14
            });
            _logger.LogInformation("📡 1. Sinal serial enviado PRIMEIRO");
        }

        // 2. DEPOIS o pacote principal
        var packageDisplayList = await GenerateDisplayPackage(placa, acesso, alphadigi);
        if (packageDisplayList != null && packageDisplayList.Any())
        {
            var package = PrepareMessage(packageDisplayList);
            serialDataList.Add(CreateSerialData(package));
            _logger.LogInformation("📦 2. Pacote principal enviado DEPOIS");
        }

        return serialDataList;
    }

    public async Task<List<SerialData>> RecieveMessageHearthbeatAlphadigi(
        string placa,
        string acesso,
        Domain.EntitiesNew.Alphadigi alphadigi)
    {
        try
        {
            _logger.LogInformation("🫀 Gerando heartbeat para display - Placa: {Placa}", placa);

            var serialDataList = new List<SerialData>();

            // 1. Gerar pacote principal
            var packageDisplayList = await GenerateDisplayPackage(placa, acesso, alphadigi);

            if (packageDisplayList != null && packageDisplayList.Any())
            {
                var package = PrepareMessage(packageDisplayList);
                serialDataList.Add(CreateSerialData(package));
            }

            // 2. ADICIONAR SINCRONIZAÇÃO DE DATA/HORA (MUITO IMPORTANTE!)
            var syncPackage = SyncDateDisplay();
            serialDataList.Add(CreateSerialData(syncPackage));

            _logger.LogInformation("✅ {Count} pacotes de heartbeat gerados", serialDataList.Count);
            return serialDataList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao gerar heartbeat para display");
            throw;
        }
    }

    public ReturnDataDisplayDTO PrepareMessage(List<CreatePackageDisplayDTO> packageDisplayList)
    {
        try
        {
            _logger.LogInformation("📦 Preparando mensagem para {Count} linhas", packageDisplayList.Count);

            foreach (var line in packageDisplayList)
            {
                _logger.LogDebug("   Linha {Linha}: '{Mensagem}' Cor: {Cor}",
                    line.Linha, line.Mensagem, line.Cor);
            }

            var package = Display.CreateMultiLinePackage(packageDisplayList);

            // Log HEX e Base64
            var hexString = BitConverter.ToString(package).Replace("-", "");
            var base64String = Convert.ToBase64String(package);

            _logger.LogInformation("📊 Pacote gerado - HEX: {Hex}", hexString);
            _logger.LogInformation("📊 Pacote gerado - Base64: {Base64}", base64String);
            _logger.LogInformation("📏 Tamanho: {Size} bytes", package.Length);

            return new ReturnDataDisplayDTO
            {
                Message = base64String,
                Size = package.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao preparar mensagem para display");
            throw;
        }
    }
    public void DebugComparePackages()
    {
        // Teste com dados conhecidos que funcionavam na versão antiga
        var testPackage = new List<CreatePackageDisplayDTO>
    {
        new CreatePackageDisplayDTO
        {
            Mensagem = "KXO9G11",
            Linha = 1,
            Cor = "yellow", // Na versão antiga, placa era amarela
            Tempo = 10,
            Estilo = 0
        },
        new CreatePackageDisplayDTO
        {
            Mensagem = "LIBERADO",
            Linha = 2,
            Cor = "green", // IMPORTANTE: deve ser verde para CADASTRADO
            Tempo = 10,
            Estilo = 0
        }
    };

        var package = Display.CreateMultiLinePackage(testPackage);
        var hex = BitConverter.ToString(package).Replace("-", "");
        var base64 = Convert.ToBase64String(package);

        _logger.LogInformation("🔍 PACOTE DE TESTE:");
        _logger.LogInformation($"   HEX: {hex}");
        _logger.LogInformation($"   Base64: {base64}");
        _logger.LogInformation($"   Tamanho: {package.Length} bytes");

        // Compare com um pacote que você sabe que funcionava
    }
    public ReturnDataDisplayDTO SyncDateDisplay()
    {
        try
        {
            // Verifique se a classe Display existe
            // Se não existir, implemente CreateTimeSyncPackage
            var package = Display.CreateTimeSyncPackage();
            LogPackage(package);

            return new ReturnDataDisplayDTO
            {
                Message = Convert.ToBase64String(package),
                Size = package.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao sincronizar data/hora do display");
            throw;
        }
    }

    public async Task<List<CreatePackageDisplayDTO>> GenerateDisplayPackage(
     string placa,
     string acesso,
     Domain.EntitiesNew.Alphadigi alphadigi)
    {
        try
        {
            _logger.LogInformation("🎯 GERANDO PACOTE DISPLAY - Placa: '{Placa}', Acesso: '{Acesso}', Linhas: {Linhas}",
       placa, acesso, alphadigi.LinhasDisplay);
            // Se não tem linhas no display, retorna null
            if (alphadigi.LinhasDisplay == 0)
                return null;

            var (displayColor, displayAcesso) = GetDisplayColorAndAcesso(placa, acesso);

            // VERIFICAÇÃO DE REPETIÇÃO (igual à versão antiga)
            var lastCamMessage = await _mensagemDisplayService.FindLastCamMensagemAsync(alphadigi.Id);

            // Caso especial: se a última mensagem da câmera foi "BEM VINDO" e estamos tentando enviar outra coisa
            if (lastCamMessage?.Placa == "BEM VINDO" && placa != "BEM VINDO")
            {
                _logger.LogDebug("⏸️  Ignorando mensagem após 'BEM VINDO'");
                return null;
            }

            // Verifica se mensagem idêntica foi enviada nos últimos 12 segundos
            var lastIdenticalMessage = await _mensagemDisplayService.FindLastMensagemAsync(
                new FindLastMessage(placa, displayAcesso, alphadigi.Id));

            var displayLines = new List<CreatePackageDisplayDTO>();

            // Adiciona placa (sempre)
            displayLines.Add(CreatePlacaDisplayDTO(placa));

            // Só adiciona mensagem de acesso se não for repetição
            if (lastIdenticalMessage == null ||
                (DateTime.Now - lastIdenticalMessage.DataHora).TotalSeconds > 12)
            {
                displayLines.Add(CreateAcessoDisplayDTO(displayAcesso, displayColor));

                // Salva no histórico
                var mensagem = new Domain.EntitiesNew.MensagemDisplay(
                    placa: placa,
                    mensagem: displayAcesso,
                    alphadigiId: alphadigi.Id,
                    dataHora: DateTime.Now,
                    prioridade: 1
                );
                await _mensagemDisplayService.SaveMensagemDisplayAsync(mensagem);
            }
            else
            {
                _logger.LogDebug("⏭️  Mensagem repetida, ignorando: {Placa} - {Acesso}",
                    placa, displayAcesso);
            }

            // Adiciona data/hora se display tem 4 linhas
            if (alphadigi.LinhasDisplay == 4)
            {
                displayLines.Add(CreateDateDisplayDTO());
                displayLines.Add(CreateTimeDisplayDTO());
            }

            foreach (var line in displayLines)
            {
                _logger.LogInformation(
                    "📝 Linha {Numero}: '{Texto}' | Cor: {Cor} | Tempo: {Tempo}s | Estilo: {Estilo}",
                    line.Linha, line.Mensagem, line.Cor, line.Tempo, line.Estilo);
            }

            return displayLines;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao gerar pacote para display");
            // Em caso de erro, pelo menos mostra algo
            return new List<CreatePackageDisplayDTO>
        {
            CreatePlacaDisplayDTO(placa),
            CreateAcessoDisplayDTO("ERRO SISTEMA", Red)
        };
        }
    }


    private async Task TrySaveToHistory(string placa, string acesso, int alphadigiId)
    {
        try
        {
            // Verifica se já existe mensagem recente para evitar salvar repetidamente
            var lastMessage = await _mensagemDisplayService.FindLastMensagemAsync(
                new FindLastMessage(placa, acesso, alphadigiId));

            // Se não tem mensagem recente (últimos 10 segundos), salva
            if (lastMessage == null || (DateTime.Now - lastMessage.DataHora).TotalSeconds > 10)
            {
                var mensagem = new Domain.EntitiesNew.MensagemDisplay(
                    placa: placa,
                    mensagem: acesso,
                    alphadigiId: alphadigiId,
                    dataHora: DateTime.Now,
                    prioridade: 1
                );

                await _mensagemDisplayService.SaveMensagemDisplayAsync(mensagem);
                _logger.LogDebug("💾 Mensagem salva no histórico: {Placa} - {Acesso}", placa, acesso);
            }
        }
        catch (Exception ex)
        {
            // Não quebra o fluxo principal se falhar ao salvar histórico
            _logger.LogWarning(ex, "⚠️  Não foi possível salvar no histórico: {Placa}", placa);
        }
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
        // EXATAMENTE igual à versão antiga
        string cor = "red";
        string acessoFormatado = acesso;

        if (string.IsNullOrEmpty(acesso))
        {
            acessoFormatado = "LIBERADO";
            cor = "green";
        }
        else
        {
            switch (acesso.ToUpper())
            {
                case "CADASTRADO":
                    cor = "green";
                    acessoFormatado = "LIBERADO";
                    break;
                case "NÃO CADASTRADO":
                case "NAO CADASTRADO":
                    acessoFormatado = "NAO CADADASTRADO";
                    break;
                case "S/VG":
                    cor = "yellow";
                    acessoFormatado = "SEM VAGA";
                    break;
            }
        }

        // CUIDADO: Na versão antiga, só muda para yellow se NÃO for "NAO CADADASTRADO"
        // e a placa for "BEM VINDO"
        if (acessoFormatado != "NAO CADADASTRADO" && placa == "BEM VINDO")
        {
            cor = "yellow";
        }

        return (cor, acessoFormatado);
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

    #endregion
}