

using Alphadigi_migration.Application.Commands.Camera;
using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Camera;

public class UpdateCameraCommandHandler : IRequestHandler<UpdateCameraCommand, bool>
{
    private readonly ICameraRepository _repository;
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateCameraCommandHandler> _logger;

    public UpdateCameraCommandHandler(
        ICameraRepository repository,
        IMediator mediator,
        ILogger<UpdateCameraCommandHandler> logger)
    {
        _repository = repository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateCameraCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando câmera: {Id}", request.Id);

        var camera = await _mediator.Send(new GetCameraByIdQuery { Id = request.Id }, cancellationToken);

        if (camera == null)
        {
            _logger.LogWarning("Câmera não encontrada: {Id}", request.Id);
            return false;
        }

        camera.AtualizarInformacoes(request.Nome, request.Ip, request.Modelo, request.Direcao);
        camera.AtualizarArea(request.IdArea);
        camera.ConfigurarFotoEvento(request.FotoEvento);

        return await _repository.UpdateAsync(camera);
    }
}
