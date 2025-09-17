

using Alphadigi_migration.Application.Commands.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Camera;

public class DeleteCameraCommandHandler : IRequestHandler<DeleteCameraCommand, bool>
{
    private readonly ICameraRepository _repository;
    private readonly ILogger<DeleteCameraCommandHandler> _logger;

    public DeleteCameraCommandHandler(
        ICameraRepository repository,
        ILogger<DeleteCameraCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCameraCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deletando câmera: {Id}", request.Id);
        
        return await _repository.DeleteAsync(request.Id);
    }
}