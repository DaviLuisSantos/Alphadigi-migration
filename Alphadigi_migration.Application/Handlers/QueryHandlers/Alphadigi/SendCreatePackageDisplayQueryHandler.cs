using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Display;
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
        if (!string.IsNullOrEmpty(placaFormatada) && placaFormatada.Length > 3)
        {
            placaFormatada = placaFormatada.Insert(3, "-");
        }

        var displayQuery = new RecieveMessageAlphadigiQuery
        {
            Linha1 = placaFormatada,
          
            Alphadigi = request.Alphadigi
        };

        return await _mediator.Send(displayQuery);
    }
}
