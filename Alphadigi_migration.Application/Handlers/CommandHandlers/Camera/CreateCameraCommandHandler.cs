using Alphadigi_migration.Application.Commands.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Camera;

public class CreateCameraCommandHandler : IRequestHandler<CreateCameraCommand, Guid>
{
    private readonly ICameraRepository _repository;
    private readonly ILogger<CreateCameraCommandHandler> _logger;

    public CreateCameraCommandHandler(
        ICameraRepository repository,
        ILogger<CreateCameraCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCameraCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando nova câmera: {Nome} ({Ip})", request.Nome, request.Ip);

        var camera = new Domain.EntitiesNew.Camera(
            request.Nome,
            request.Ip,
            request.IdArea,
            request.Modelo,
            request.Direcao,
            request.FotoEvento);

        return await _repository.AddAsync(camera);
    }
}