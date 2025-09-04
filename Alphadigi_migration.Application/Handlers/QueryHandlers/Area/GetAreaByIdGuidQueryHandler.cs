using Alphadigi_migration.Application.Queries.Area;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Area;

public class GetAreaByIdGuidQueryHandler : IRequestHandler<GetAreaByIdGuidQuery, 
                                                           Domain.EntitiesNew.Area>
{

    private readonly ILogger<GetAreaByIdGuidQueryHandler> _logger;
    private readonly IAreaRepository _repository;

    public GetAreaByIdGuidQueryHandler(ILogger<GetAreaByIdGuidQueryHandler> logger, 
                                       IAreaRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Domain.EntitiesNew.Area> Handle(GetAreaByIdGuidQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando área por GUID: {Id}", request.Id);

        return await _repository.GetByIdAsync(request.Id);
    }
}
    

