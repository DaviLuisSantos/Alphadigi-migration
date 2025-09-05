

using Alphadigi_migration.Application.Queries.Area;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Area;

public class GetAreaByIdQueryHandler : IRequestHandler<GetAreaByIdQuery, Domain.EntitiesNew.Area>
{
    private readonly IAreaRepository _repository;
    private readonly ILogger<GetAreaByIdQueryHandler> _logger;

    public GetAreaByIdQueryHandler(IAreaRepository repository, 
                                   ILogger<GetAreaByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Area> Handle(GetAreaByIdQuery request, 
                                                CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando área por ID: {Id}", request.id);
        return await _repository.GetByIdAsync(request.id);
    }
}
