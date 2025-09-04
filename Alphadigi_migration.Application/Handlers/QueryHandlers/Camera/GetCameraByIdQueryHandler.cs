using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Camera;

public class GetCameraByIdQueryHandler : IRequestHandler<GetCameraByIdQuery, Domain.EntitiesNew.Camera>
{
    private readonly ICameraRepository _repository;
    private readonly ILogger<GetCameraByIdQueryHandler> _logger;

    public GetCameraByIdQueryHandler(
        ICameraRepository repository,
        ILogger<GetCameraByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Camera> Handle(GetCameraByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando câmera por ID: {Id}", request.Id);
        return await _repository.GetByIdAsync(request.Id);
    }
}
