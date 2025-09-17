using Alphadigi_migration.Application.Commands.Display;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Display;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Service;

// Application/Services/DisplayAppService.cs
public class DisplayAppService
{
    private readonly IMediator _mediator;
    private readonly IDisplayProtocolService _displayProtocolService;
    private readonly ILogger<DisplayAppService> _logger;

    public DisplayAppService(
        IMediator mediator,
        IDisplayProtocolService displayProtocolService,
        ILogger<DisplayAppService> logger)
    {
        _mediator = mediator;
        _displayProtocolService = displayProtocolService;
        _logger = logger;
    }

    public async Task<List<SerialData>> RecieveMessageAlphadigi(string placa, 
                                                                string acesso, 
                                                                int alphadigiId)
    {
        var command = new CreateDisplayPackageCommand(placa, acesso, alphadigiId);
        return await _mediator.Send(command);
    }

    public async Task<List<SerialData>> RecieveMessageHearthbeatAlphadigi(string placa, 
                                                                          string acesso, 
                                                                          int alphadigiId)
    {
        var command = new CreateHeartbeatDisplayPackageCommand(placa, acesso, alphadigiId);
        return await _mediator.Send(command);
    }

    public ReturnDataDisplayDTO PrepareMessage(List<CreatePackageDisplayDTO> packageDisplayList)
    {
        var package = _displayProtocolService.CreateMultiLinePackage(packageDisplayList);
        LogPackage(package);
        return new ReturnDataDisplayDTO
        {
            Message = Convert.ToBase64String(package),
            Size = package.Length
        };
    }

    public ReturnDataDisplayDTO SyncDateDisplay()
    {
        var package = _displayProtocolService.CreateTimeSyncPackage();
        LogPackage(package);
        return new ReturnDataDisplayDTO
        {
            Message = Convert.ToBase64String(package),
            Size = package.Length
        };
    }

    private void LogPackage(byte[] package)
    {
        var hexString = BitConverter.ToString(package).Replace("-", "");
        _logger.LogInformation($"PACOTE GERADO: {hexString}");
    }
}
