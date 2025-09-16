using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.PlacaLida;

public class CreatePlacaLidaCommandHandler : IRequestHandler<CreatePlacaLidaCommand, Domain.EntitiesNew.PlacaLida>
{
    private readonly IPlacaLidaRepository _repository;
    private readonly ILogger<CreatePlacaLidaCommandHandler> _logger;

    public CreatePlacaLidaCommandHandler(
        IPlacaLidaRepository repository,
        ILogger<CreatePlacaLidaCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.PlacaLida> Handle(
        CreatePlacaLidaCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando registro de placa lida: {Placa}", request.Placa);
        _logger.LogInformation("Criando registro de placa lida: {Placa}", request.Placa);
        _logger.LogInformation("Placa é nula?: {IsNull}", request.Placa == null);
        _logger.LogInformation("Placa é vazia?: {IsEmpty}", string.IsNullOrEmpty(request.Placa));

        if (string.IsNullOrWhiteSpace(request.Placa))
        {
            _logger.LogError("PLACA É NULA OU VAZIA! Valor: '{Placa}'", request.Placa);
            throw new ArgumentException("Placa não pode ser nula ou vazia");
        }


        var placaLida = new Domain.EntitiesNew.PlacaLida
        (
            alphadigiId: request.AlphadigiId,
            placa: request.Placa,
            dataHora: request.DataHora,
            areaId: request.AreaId,
            placaImg: request.PlacaImg ?? "N/A",
            carroImg: request.CarroImg ?? "N/A",
            acesso: "PENDENTE",
            liberado :false,
            processado: false
        );
        _logger.LogInformation("Registro de placa lida criado com sucesso para a placa: {Placa}", request.Placa);

        return await _repository.AddAsync(placaLida);
    }
}