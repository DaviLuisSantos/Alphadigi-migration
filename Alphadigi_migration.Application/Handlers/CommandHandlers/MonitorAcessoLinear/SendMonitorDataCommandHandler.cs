using Alphadigi_migration.Application.Commands.MonitorAcessoLinear;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.MonitorAcessoLinear;

public class SendMonitorDataCommandHandler : IRequestHandler<SendMonitorDataCommand, bool>
{
    private readonly IMonitorAcessoLinear _monitorAcessoLinear;
    private readonly ILogger<SendMonitorDataCommandHandler> _logger;

    public SendMonitorDataCommandHandler(
        IMonitorAcessoLinear monitorAcessoLinear,
        ILogger<SendMonitorDataCommandHandler> logger)
    {
        _monitorAcessoLinear = monitorAcessoLinear;
        _logger = logger;
    }

    public async Task<bool> Handle(SendMonitorDataCommand request, CancellationToken cancellationToken)
    {
        if (request?.Dados == null)
        {
            _logger.LogWarning("Comando SendMonitorDataCommand recebido com Dados nulo");
            return false;
        }

        try
        {
            _logger.LogInformation(
                "Enviando dados para monitor | Placa: {Placa} | IP: {Ip} | Acesso: {Acesso} | Hora: {Hora}",
                request.Dados.Placa,
                request.Dados.Ip,
                request.Dados.Acesso,
                request.Dados.HoraAcesso
            );

            // Chama o monitor atualizado
            return await _monitorAcessoLinear.DadosVeiculo(request.Dados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar dados para monitor");
            return false;
        }
    }
}
