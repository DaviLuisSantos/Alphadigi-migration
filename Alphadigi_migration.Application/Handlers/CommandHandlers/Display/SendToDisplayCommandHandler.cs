using Alphadigi_migration.Application.Commands.Display;
using Alphadigi_migration.Application.Service;
using MediatR;
using Microsoft.Extensions.Logging;

public class SendToDisplayCommandHandler : IRequestHandler<SendToDisplayCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendToDisplayCommandHandler> _logger;
    private readonly DisplayService _displayService;

    public SendToDisplayCommandHandler(
        IMediator mediator,
        ILogger<SendToDisplayCommandHandler> logger,
        DisplayService displayService)
    {
        _mediator = mediator;
        _logger = logger;
        _displayService = displayService;
    }

    public async Task Handle(SendToDisplayCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("📺 GERANDO DADOS PARA DISPLAY - Placa: {Placa}, Acesso: {Acesso}",
                request.Placa, request.Acesso);

            // Apenas gera os dados - o AlphaDigi vai buscá-los
            var serialData = await _displayService.RecieveMessageAlphadigi(
                request.Placa,
                request.Acesso,
                request.Alphadigi);

            _logger.LogInformation("✅ Dados gerados para display: {Count} pacotes",
                serialData?.Count ?? 0);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao gerar dados para display");
            throw;
        }
    }
}