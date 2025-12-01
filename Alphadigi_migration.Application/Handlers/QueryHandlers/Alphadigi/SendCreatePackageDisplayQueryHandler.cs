using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Display;
using Alphadigi_migration.Application.Service; // Adicione esta using
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class SendCreatePackageDisplayQueryHandler : IRequestHandler<SendCreatePackageDisplayQuery,
                                                                    List<Domain.DTOs.Alphadigi.SerialData>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendCreatePackageDisplayQueryHandler> _logger;
    private readonly DisplayService _displayService; // Adicione esta linha

    public SendCreatePackageDisplayQueryHandler(
        IMediator mediator,
        ILogger<SendCreatePackageDisplayQueryHandler> logger,
        DisplayService displayService) // Adicione no construtor
    {
        _mediator = mediator;
        _logger = logger;
        _displayService = displayService;
    }

    public async Task<List<Domain.DTOs.Alphadigi.SerialData>> Handle(
        SendCreatePackageDisplayQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("INICIANDO SendCreatePackageDisplayQuery - Placa: {Placa}",
            request.Veiculo?.Placa);

        try
        {
            if (request.Veiculo == null)
            {
                _logger.LogError("VEÍCULO É NULL!");
                return new List<SerialData>();
            }

            if (string.IsNullOrEmpty(request.Acesso))
            {
                _logger.LogWarning("ACESSO ESTÁ VAZIO!");
                request.Acesso = "ACESSO INDEFINIDO";
            }

            // ***** USE O DISPLAY SERVICE CORRETO *****
            var serialData = await _displayService.RecieveMessageAlphadigi(
                request.Veiculo.Placa ?? "SEM PLACA",
                request.Acesso,
                request.Alphadigi);

            _logger.LogInformation("✅ Pacotes gerados: {Count}", serialData?.Count ?? 0);

            return serialData ?? new List<SerialData>();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERRO em SendCreatePackageDisplayQuery");
            throw;
        }
    }
}