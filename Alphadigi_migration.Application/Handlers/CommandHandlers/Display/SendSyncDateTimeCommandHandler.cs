//// SendSyncDateTimeCommandHandler.cs
//using Alphadigi_migration.Application.Commands.Display;
//using Alphadigi_migration.Application.Service;
//using MediatR;
//using Microsoft.Extensions.Logging;

//namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Display;

//public class SendSyncDateTimeCommandHandler : IRequestHandler<SendSyncDateTimeCommand>
//{
//    private readonly ILogger<SendSyncDateTimeCommandHandler> _logger;
//    private readonly IDisplayService _displayService; // Você precisa criar esta interface/service

//    public SendSyncDateTimeCommandHandler(
//        ILogger<SendSyncDateTimeCommandHandler> logger,
//        IDisplayService displayService)
//    {
//        _logger = logger;
//        _displayService = displayService;
//    }

//    public async Task Handle(SendSyncDateTimeCommand request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            _logger.LogInformation("🔄 Sincronizando data/hora no display");

//            // Chama o serviço para sincronizar data/hora
//            var syncPackage = _displayService.CreateSyncDateTimePackage();

//            // Envia para o display (depende da sua implementação de comunicação)
//            await SendToDisplay(request.Alphadigi, syncPackage);

//            _logger.LogInformation("✅ Data/hora sincronizada");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "❌ Erro ao sincronizar data/hora");
//            throw;
//        }
//    }

//    private async Task SendToDisplay(Domain.EntitiesNew.Alphadigi alphadigi, byte[] package)
//    {
//        // Implemente o envio para o display aqui
//        // Isso depende de como você se comunica com o totem
//    }
//}