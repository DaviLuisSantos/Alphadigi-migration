using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;

public class SendMonitorAcessoLinearCommandHandler : IRequestHandler<SendMonitorAcessoLinearCommand, bool>
{
    private readonly IMonitorAcessoLinear _monitorAcessoLinear;
    private readonly ILogger<SendMonitorAcessoLinearCommandHandler> _logger;

    public SendMonitorAcessoLinearCommandHandler(
        IMonitorAcessoLinear monitorAcessoLinear,
        ILogger<SendMonitorAcessoLinearCommandHandler> logger)
    {
        _monitorAcessoLinear = monitorAcessoLinear;
        _logger = logger;
    }

    public async Task<bool> Handle(SendMonitorAcessoLinearCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando envio de dados para o Monitor Acesso Linear.");
        try
        {
            // Verifique se o objeto Veiculo e o IP da Câmera estão disponíveis.
            if (request.IpCamera == null || string.IsNullOrEmpty(request.IpCamera))
            {
                _logger.LogError("Dados de requisição inválidos: Veículo ou IP da câmera ausentes.");
                return false;
            }

            // Chame diretamente o método da classe de serviço para enviar o broadcast,
            // que agora contém a lógica de formatação da string e envio UDP.
            await _monitorAcessoLinear.DadosVeiculo(request.DadosVeiculo);

            _logger.LogInformation("Dados de acesso para a placa {Placa} enviados com sucesso para o monitor.", request.DadosVeiculo.Placa);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar dados para monitor de acesso linear para a placa {Placa}.", request.DadosVeiculo.Placa);
            return false;
        }
    }
}