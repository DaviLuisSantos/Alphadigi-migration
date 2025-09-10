using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Camera;

public class GetAllCamerasQueryHandler : IRequestHandler<GetAllCamerasQuery, 
                                                         List<Domain.EntitiesNew.Camera>>
{
    private readonly ICameraRepository _repository;
    private readonly ILogger<GetAllCamerasQueryHandler> _logger;

    public GetAllCamerasQueryHandler(
        ICameraRepository repository,
        ILogger<GetAllCamerasQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Domain.EntitiesNew.Camera>> Handle(GetAllCamerasQuery request, 
                                                              CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando todas as câmeras");
        return await _repository.GetAllAsync();
    }
}
