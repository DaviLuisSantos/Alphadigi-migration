using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Camera;

public class GetCameraByIpQueryHandler : IRequestHandler<GetCameraByIpQuery, Domain.EntitiesNew.Camera>
{
    private readonly ICameraRepository _repository;
    private readonly ILogger<GetCameraByIpQueryHandler> _logger;

    public GetCameraByIpQueryHandler(
        ICameraRepository repository,
        ILogger<GetCameraByIpQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Camera> Handle(GetCameraByIpQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando câmera por IP: {Ip}", request.Ip);
        return await _repository.GetByIpAsync(request.Ip);
    }
}