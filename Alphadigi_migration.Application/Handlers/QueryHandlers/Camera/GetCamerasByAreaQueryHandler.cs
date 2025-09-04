using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Camera;

public class GetCamerasByAreaQueryHandler : IRequestHandler<GetCamerasByAreaQuery, List<Domain.EntitiesNew.Camera>>
{
    private readonly ICameraRepository _repository;
    private readonly ILogger<GetCamerasByAreaQueryHandler> _logger;

    public GetCamerasByAreaQueryHandler(
        ICameraRepository repository,
        ILogger<GetCamerasByAreaQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Domain.EntitiesNew.Camera>> Handle(GetCamerasByAreaQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando câmeras da área: {AreaId}", request.AreaId);
        return await _repository.GetByAreaIdAsync(request.AreaId);
    }
}
