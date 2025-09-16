using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Display;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
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
        _logger.LogInformation("INICIANDO SendCreatePackageDisplayQuery - Placa: {Placa}", request.Veiculo.Placa);

        string placaFormatada = request.Veiculo.Placa;
        try
        {
            if (request.Veiculo == null)
            {
                _logger.LogError(" VEÍCULO É NULL!");
                return new List<SerialData>();
            }

            if (string.IsNullOrEmpty(request.Acesso))
            {
                _logger.LogWarning(" ACESSO ESTÁ VAZIO!");
                request.Acesso = "ACESSO INDEFINIDO";
            }

            var veiculoDataQuery = new PrepareVeiculoDataStringQuery { Veiculo = request.Veiculo };
            var dadosCompletos = await _mediator.Send(veiculoDataQuery, cancellationToken);

            _logger.LogInformation("Dados completos do veículo: {Dados}", dadosCompletos);

            var mensagemDisplay = $"{placaFormatada} | {dadosCompletos} | {request.Acesso}";
            _logger.LogInformation(" Mensagem do display: {Mensagem}", mensagemDisplay);

            string mensagemAviso = request.Acesso.Contains("NEGADO") ?
           $"ACESSO NEGADO: {placaFormatada}" :
           $"ACESSO LIBERADO: {placaFormatada}";

            var displayQuery = new RecieveMessageAlphadigiQuery
            {
                Linha1 = mensagemDisplay,
                Alphadigi = request.Alphadigi,
                Tipo = "lista",
            };

            _logger.LogInformation("Enviando para display: {Mensagem}", mensagemDisplay);
            return await _mediator.Send(displayQuery, cancellationToken);



        } catch (Exception ex)
        {
            _logger.LogError(ex, " ERRO em SendCreatePackageDisplayQuery");
            throw;
        }

        
       
    }
}
