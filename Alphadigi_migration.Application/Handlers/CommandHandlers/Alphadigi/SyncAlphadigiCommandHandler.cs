using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class SyncAlphadigiCommandHandler : IRequestHandler<SyncAlphadigiCommand, bool>
{
    private readonly ILogger<SyncAlphadigiCommandHandler> _logger;
    private readonly IAlphadigiRepository _repository;

    public async Task<bool> Handle(SyncAlphadigiCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.SyncAlphadigi();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar Alphadigi");
            throw;
        }
    }
}
