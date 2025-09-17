using Alphadigi_migration.Application.Commands.Condominio;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Condominio;

public class SyncCondominioCommandHandler : IRequestHandler<SyncCondominioCommand, bool>
{
    private readonly ICondominioRepository _repository;
    private readonly ILogger<SyncCondominioCommandHandler> _logger;

    public SyncCondominioCommandHandler(ICondominioRepository repository, 
                                        ILogger<SyncCondominioCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(SyncCondominioCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sincronizando condomínio");
        return await _repository.SyncCondominioAsync();
    }


}
