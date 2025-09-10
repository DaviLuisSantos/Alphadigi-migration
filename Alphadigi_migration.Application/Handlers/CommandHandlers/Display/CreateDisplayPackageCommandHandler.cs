using Alphadigi_migration.Application.Commands.Display;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.ValueObjects;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Display;


public class CreateDisplayPackageCommandHandler : IRequestHandler<CreateDisplayPackageCommand, 
                                                                  List<SerialData>>
{
    private readonly IMensagemDisplayRepository _mensagemDisplayRepository;
    private readonly IAlphadigiRepository _alphadigiRepository;
    private readonly IDisplayProtocolService _displayProtocolService;
    private readonly ILogger<CreateDisplayPackageCommandHandler> _logger;
    private readonly IMapper _mapper;

    public CreateDisplayPackageCommandHandler(
        IMensagemDisplayRepository mensagemDisplayRepository,
        IAlphadigiRepository alphadigiRepository,
        IDisplayProtocolService displayProtocolService,
        ILogger<CreateDisplayPackageCommandHandler> logger,
        IMapper mapper)
    {
        _mensagemDisplayRepository = mensagemDisplayRepository;
        _alphadigiRepository = alphadigiRepository;
        _displayProtocolService = displayProtocolService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<SerialData>> Handle(CreateDisplayPackageCommand request, 
                                               CancellationToken cancellationToken)
    {
        var alphadigi = await _alphadigiRepository.GetById(request.AlphadigiId);
        if (alphadigi == null)
            throw new Exception($"Alphadigi com ID {request.AlphadigiId} não encontrado");

        var serialDataList = new List<SerialData>();
        var packageDisplayList = await PrepareCreatePackage(request.Placa, request.Acesso, alphadigi);

        if (packageDisplayList != null && packageDisplayList.Any())
        {
            var package = PrepareMessage(packageDisplayList);
            serialDataList.Add(CreateSerialData(package));
        }

        if (request.Acesso == DisplayConstants.Cadastrado)
        {
            serialDataList.Add(new SerialData
            {
                serialChannel = 0,
                data = DisplayConstants.SinalSerialData,
                dataLen = DisplayConstants.SinalSerialDataLen
            });
        }

        return serialDataList;
    }

    #region Private Helper Methods

    private SerialData CreateSerialData(ReturnDataDisplayDTO package)
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

    private async Task<List<CreatePackageDisplayDTO>> PrepareCreatePackage(string placa, 
                                                                           string acesso, 
                                                                           Domain.EntitiesNew.Alphadigi alphadigi)
    {
        if (alphadigi.LinhasDisplay == 0 && placa == DisplayConstants.WelcomeMessage)
            return null;

        var (displayColor, displayAcesso) = GetDisplayColorAndAcesso(placa, acesso);
        var serialDataList = new List<CreatePackageDisplayDTO>
        {
            CreatePlacaDisplayDTO(placa)
        };

        var lastMessage = await _mensagemDisplayRepository.FindLastMensagemAsync(placa, displayAcesso, alphadigi.Id);
        var lastCamMessage = await _mensagemDisplayRepository.FindLastCamMensagemAsync(alphadigi.Id);

        if (ShouldAddAcessoLine(lastMessage, lastCamMessage, placa))
        {
            serialDataList.Add(CreateAcessoDisplayDTO(displayAcesso, displayColor));
            await SaveMensagemDisplayAsync(placa, displayAcesso, alphadigi.Id);
        }
        else if (lastCamMessage?.Placa == DisplayConstants.WelcomeMessage)
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

    private static (string Color, string Acesso) GetDisplayColorAndAcesso(string placa, string acesso)
    {
        return acesso switch
        {
            "" or DisplayConstants.Cadastrado => (DisplayConstants.Green, DisplayConstants.Liberado),
            DisplayConstants.NaoCadastrado => (DisplayConstants.Red, DisplayConstants.NaoCadastradoFixed),
            DisplayConstants.SemVaga => (DisplayConstants.Yellow, DisplayConstants.SemVagaFixed),
            _ => (placa == DisplayConstants.WelcomeMessage ? DisplayConstants.Yellow : DisplayConstants.Red,
                  acesso == "" ? DisplayConstants.Liberado : acesso)
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
            Cor = placa == DisplayConstants.WelcomeMessage ? DisplayConstants.Green : DisplayConstants.Yellow,
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
            Cor = DisplayConstants.Red,
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
            Cor = DisplayConstants.Yellow,
            Tempo = 0,
            Estilo = 0
        };
    }

    private static bool ShouldAddAcessoLine(Domain.EntitiesNew.MensagemDisplay lastMessage, Domain.EntitiesNew.MensagemDisplay lastCamMessage, string placa)
    {
        return lastMessage == null ||
               (lastCamMessage?.Id == null || (lastCamMessage != null && lastCamMessage.Id != lastMessage.Id && lastCamMessage.Placa != placa));
    }

    private async Task SaveMensagemDisplayAsync(string placa, string acesso, int alphadigiId)
    {
        var mensagem = new Domain.EntitiesNew.MensagemDisplay
        (
            placa: placa,
            mensagem: acesso,
            dataHora: DateTime.Now,
            alphadigiId: alphadigiId
        );
      
        await _mensagemDisplayRepository.SaveMensagemDisplayAsync(mensagem);
    }

    private ReturnDataDisplayDTO PrepareMessage(List<CreatePackageDisplayDTO> packageDisplayList)
    {
        var package = _displayProtocolService.CreateMultiLinePackage(lines: packageDisplayList,
        voiceText: null,         
        saveToFlash: false);


        LogPackage(package);
        return new ReturnDataDisplayDTO
        {
            Message = Convert.ToBase64String(package),
            Size = package.Length
        };
    }

    #endregion
}
