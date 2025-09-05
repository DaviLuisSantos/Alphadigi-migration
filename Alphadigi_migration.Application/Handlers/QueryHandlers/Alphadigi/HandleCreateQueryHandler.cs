using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Veiculo;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class HandleCreateQueryHandler : IRequestHandler<HandleCreateQuery, Domain.DTOs.Alphadigi.AddWhiteListDTO>
{
    private readonly IMediator _mediator;
    private readonly ILogger<HandleCreateQueryHandler> _logger;

    public HandleCreateQueryHandler(
        IMediator mediator,
        ILogger<HandleCreateQueryHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Domain.DTOs.Alphadigi.AddWhiteListDTO> Handle(HandleCreateQuery request, CancellationToken cancellationToken)
    {
        var alphadigi = request.Alphadigi;
        Guid ultimoId = alphadigi.UltimoId ?? Guid.Empty;

        // Buscar veículos usando query
        var veiculosQuery = new GetVeiculosSendQuery { UltimoId = ultimoId };
        var veiculosEnvio = await _mediator.Send(veiculosQuery, cancellationToken);

        if (veiculosEnvio.Count == 0)
        {
            return null;
        }

        // Atualizar o UltimoId com o ID do último veículo enviado
        alphadigi.AtualizarUltimoId(veiculosEnvio.Max(item => item.Id));


        var envio = new Domain.DTOs.Alphadigi.AddWhiteListDTO
        {
            AddWhiteList = new Domain.DTOs.Alphadigi.AddWhiteList
            {
                Add_data = veiculosEnvio.Select(item => new Domain.DTOs.Alphadigi.AddData
                {
                    Carnum = item.Placa,
                    Startime = "20200718155220",
                    Endtime = "30990718155220"
                }).ToList()
            }
        };

        // Atualizar Alphadigi
        var updateCommand = new UpdateAlphadigiEntityCommand { Alphadigi = alphadigi };
        await _mediator.Send(updateCommand, cancellationToken);

        // Salvar em arquivo (mantendo a funcionalidade original)
        var filePath = "responseCreateHb.json";
        var jsonResult = JsonSerializer.Serialize(envio);
        await File.WriteAllTextAsync(filePath, jsonResult);

        return envio;
    }
}
