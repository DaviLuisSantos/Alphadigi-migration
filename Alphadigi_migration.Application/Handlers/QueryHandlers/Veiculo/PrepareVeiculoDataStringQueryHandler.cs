using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Veiculo;
public class PrepareVeiculoDataStringQueryHandler : IRequestHandler<PrepareVeiculoDataStringQuery, string>
{
    private readonly ILogger<PrepareVeiculoDataStringQueryHandler> _logger;

    public PrepareVeiculoDataStringQueryHandler(ILogger<PrepareVeiculoDataStringQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<string> Handle(PrepareVeiculoDataStringQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Preparando string de dados do veículo: {request.Veiculo.Placa}");

            if (request.Veiculo == null)
            {
                _logger.LogWarning("Veículo é nulo na query PrepareVeiculoDataStringQuery");
                return Task.FromResult("VEÍCULO NÃO ENCONTRADO");
            }
            _logger.LogInformation("📊 Dados do veículo - Modelo: {Modelo}, Cor: {Cor}, Proprietário: {Proprietario}",
            request.Veiculo.Modelo ?? "NULL",
              request.Veiculo.Placa ?? "NULL",
             request.Veiculo.Unidade ?? "NULL");

            // Formatar os dados do veículo em uma string
            _logger.LogInformation("Dados do veículo - Modelo: {Modelo}, Cor: {Cor}, Proprietário: {Proprietario}",
              request.Veiculo.Modelo,
                 request.Veiculo.Placa,
                request.Veiculo.Unidade);

            var dataString = $"{request.Veiculo.Placa} | " +
                        $"{request.Veiculo.Modelo ?? "N/A"} | " +
                        $"{request.Veiculo.Cor ?? "N/A"} | " +
                        $"{request.Veiculo.Unidade ?? "N/A"}";

            _logger.LogInformation($"String de dados preparada: {dataString}");

            return Task.FromResult(dataString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao preparar string de dados do veículo");
            return Task.FromResult("ERRO AO PROCESSAR DADOS DO VEÍCULO");
        }
    }

   
}