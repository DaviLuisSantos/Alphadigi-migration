using Alphadigi_migration.Application.Commands.Camera;
using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Camera;

public class AtivarCameraCommandHandler : IRequestHandler<AtivarCameraCommand, bool>
{
    private readonly ICameraRepository _repository;
    private readonly IMediator _mediator;
    private readonly ILogger<AtivarCameraCommandHandler> _logger;

    public AtivarCameraCommandHandler(
        ICameraRepository repository,
        IMediator mediator,
        ILogger<AtivarCameraCommandHandler> logger)
    {
        _repository = repository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(AtivarCameraCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ativando câmera: {Id}", request.Id);

        var camera = await _mediator.Send(new GetCameraByIdQuery { Id = request.Id }, cancellationToken);

        if (camera == null)
        {
            _logger.LogWarning("Câmera não encontrada: {Id}", request.Id);
            return false;
        }

        camera.Ativar();
        return await _repository.UpdateAsync(camera);
    }
}