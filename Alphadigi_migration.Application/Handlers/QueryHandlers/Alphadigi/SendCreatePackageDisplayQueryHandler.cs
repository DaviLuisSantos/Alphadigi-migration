using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Display;
using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class SendCreatePackageDisplayQueryHandler : IRequestHandler<SendCreatePackageDisplayQuery, 
                                                                    List<Domain.DTOs.Alphadigi.SerialData>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SendCreatePackageDisplayQueryHandler> _logger;

    public SendCreatePackageDisplayQueryHandler(IMediator mediator, 
                                                ILogger<SendCreatePackageDisplayQueryHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<List<Domain.DTOs.Alphadigi.SerialData>> Handle(SendCreatePackageDisplayQuery request, 
                                                                     CancellationToken cancellationToken)
    {


        string placaFormatada = request.Veiculo.Placa;
      

        // 2. Buscar informações completas do veículo (modelo, cor, etc.)
        var veiculoDataQuery = new PrepareVeiculoDataStringQuery { Veiculo = request.Veiculo };
        var dadosCompletos = await _mediator.Send(veiculoDataQuery, cancellationToken);

        _logger.LogInformation("Dados completos do veículo: {Dados}", dadosCompletos);

        var mensagemDisplay = $"{placaFormatada} | {dadosCompletos} | {request.Acesso}";
        var displayQuery = new RecieveMessageAlphadigiQuery
        {
            Linha1 = placaFormatada,
          
            Alphadigi = request.Alphadigi
        };

        return await _mediator.Send(displayQuery);
    }
}
