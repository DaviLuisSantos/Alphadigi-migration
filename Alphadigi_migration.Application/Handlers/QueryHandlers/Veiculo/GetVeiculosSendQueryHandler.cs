using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Veiculo;

public class GetVeiculosSendQueryHandler : IRequestHandler<GetVeiculosSendQuery,
                                                           List<Domain.DTOs.Veiculos.VeiculoInfoSendAlphadigi>>
{
    private readonly IVeiculoService _veiculoService;
    private readonly ILogger<GetVeiculosSendQueryHandler> _logger;

    public GetVeiculosSendQueryHandler(
        IVeiculoService veiculoService,
        ILogger<GetVeiculosSendQueryHandler> logger)
    {
        _veiculoService = veiculoService;
        _logger = logger;
    }

    public async Task<List<Domain.DTOs.Veiculos.VeiculoInfoSendAlphadigi>> Handle(
        GetVeiculosSendQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 GetVeiculosSendQueryHandler - UltimoId: {UltimoId}", request.UltimoId);

        try
        {
            var veiculos = await _veiculoService.GetVeiculosSend(request.UltimoId);

            _logger.LogInformation("📊 Encontrados {Count} veículos a partir do ID: {UltimoId}",
                veiculos.Count, request.UltimoId);

            if (veiculos.Any())
            {
                _logger.LogInformation("   Primeiros 5 veículos:");
                foreach (var veiculo in veiculos.Take(5))
                {
                    _logger.LogInformation("   🚗 ID: {Id}, Placa: {Placa}",
                        veiculo.Id, veiculo.Placa);
                }
            }
            else
            {
                _logger.LogInformation("   ℹ️ Nenhum veículo encontrado");
            }

            return veiculos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro em GetVeiculosSendQueryHandler");
            throw;
        }
    }
}