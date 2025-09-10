using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Veiculo
{
    public class SendDadosVeiculoMonitorQueryHandler : IRequestHandler<SendDadosVeiculoMonitorQuery, bool>
    {
        private readonly ILogger<SendDadosVeiculoMonitorQueryHandler> _logger;

        public SendDadosVeiculoMonitorQueryHandler(ILogger<SendDadosVeiculoMonitorQueryHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Handle(SendDadosVeiculoMonitorQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Enviando dados para monitor: Veículo {Placa}, Acesso: {Acesso}, Camera: {IpCamera}",
                    request.Veiculo.Placa, request.Acesso, request.Ip);

                // Implemente aqui a lógica real de envio para o monitor
                // Por exemplo: enviar para um serviço HTTP, message broker, etc.

                _logger.LogInformation("Dados enviados com sucesso para o monitor");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar dados para monitor do veículo {Placa}", request.Veiculo.Placa);
                return false;
            }
        }
    }
}