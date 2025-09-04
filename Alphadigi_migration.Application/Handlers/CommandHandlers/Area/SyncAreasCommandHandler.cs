using Alphadigi_migration.Application.Commands.Area;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Area;

public class SyncAreasCommandHandler : IRequestHandler<SyncAreasCommand, bool>
{
    private readonly IAreaRepository _repository;
    private readonly ILogger<SyncAreasCommandHandler> _logger;

    public SyncAreasCommandHandler(IAreaRepository repository, 
                                   ILogger<SyncAreasCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(SyncAreasCommand request, 
                             CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sincronizando áreas");
        return await _repository.SyncAreasAsync();
    }
}
