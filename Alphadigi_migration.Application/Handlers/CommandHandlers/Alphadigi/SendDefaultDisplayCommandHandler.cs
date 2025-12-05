using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class SendDefaultDisplayCommandHandler : IRequestHandler<SendDefaultDisplayCommand, List<SerialData>>
{
    private readonly DisplayService _displayService;
    private readonly ILogger<SendDefaultDisplayCommandHandler> _logger;

    public SendDefaultDisplayCommandHandler(
        DisplayService displayService,
        ILogger<SendDefaultDisplayCommandHandler> logger)
    {
        _displayService = displayService;
        _logger = logger;
    }

    public async Task<List<SerialData>> Handle(SendDefaultDisplayCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"📺 Enviando display padrão para câmera: {request.Alphadigi.Ip}");

        // Envia "BEM VINDO" como padrão
        return await _displayService.RecieveMessageHearthbeatAlphadigi(
            "BEM VINDO",
            "CADASTRADO",
            request.Alphadigi);
    }
}