using Alphadigi_migration.Application.Commands.SendUdpBroadcast;
using Alphadigi_migration.Domain.DTOs.SystemBroadcast;
using Alphadigi_migration.Domain.DTOs.VehicleBroadcast;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.SendUdpBroadcast;

public class SendUdpBroadcastCommandHandler : IRequestHandler<SendUdpBroadcastCommand>
{
    private readonly IUdpBroadcastService _udpBroadcastService;
    private readonly ILogger<SendUdpBroadcastCommandHandler> _logger;

    public SendUdpBroadcastCommandHandler(
        IUdpBroadcastService udpBroadcastService,
        ILogger<SendUdpBroadcastCommandHandler> logger)
    {
        _udpBroadcastService = udpBroadcastService;
        _logger = logger;
    }

    public async Task Handle(SendUdpBroadcastCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(" RECEBIDO COMANDO BROADCAST - Tipo: {Tipo}",
            request.Data.GetType().Name);

        try
        {
            if (request.IsVehicleData && request.Data is VehicleBroadcastDTO vehicleData)
            {
                _logger.LogInformation(" ENVIANDO VEICULO - Placa: {Placa}", vehicleData.Placa);
                await _udpBroadcastService.SendVehicleDataAsync(vehicleData, request.IpAddress);
            }
            else if (request.Data is SystemBroadcastDTO systemData)
            {
                _logger.LogInformation(" ENVIANDO SISTEMA");
                await _udpBroadcastService.SendSystemDataAsync(systemData, request.IpAddress);
            }
            else
            {
                _logger.LogWarning("❌ TIPO NAO SUPORTADO: {Type}", request.Data.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar comando de broadcast");
            throw;
        }
    }
}