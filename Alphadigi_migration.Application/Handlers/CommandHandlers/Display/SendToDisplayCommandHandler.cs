using Alphadigi_migration.Application.Commands.Display;
using Alphadigi_migration.Application.Commands.SendUdpBroadcast;
using Alphadigi_migration.Application.Queries.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Display;

public class SendToDisplayCommandHandler : IRequestHandler<SendToDisplayCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendToDisplayCommandHandler> _logger;

    public SendToDisplayCommandHandler(IMediator mediator, ILogger<SendToDisplayCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(SendToDisplayCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("🖥️ ENVIANDO PARA DISPLAY - Placa: {Placa}, Acesso: {Acesso}",
                request.Placa, request.Acesso);



            var displayQuery = new SendCreatePackageDisplayQuery
            {
                Veiculo = request.Veiculo,
                Acesso = request.Acesso,
                Alphadigi = request.Alphadigi
            };

            var messageDisplay = await _mediator.Send(displayQuery, cancellationToken);

            _logger.LogInformation("✅ Dados enviados para display: {Mensagens} mensagens",
                messageDisplay.Count);
          
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao enviar para display");
            throw;
        }
    }
}
