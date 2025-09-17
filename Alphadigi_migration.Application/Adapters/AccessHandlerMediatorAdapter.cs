using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Adapters;

public class AccessHandlerMediatorAdapter : IRequestHandler<HandleAccessCommand, (bool ShouldReturn, string Acesso)>
{
    private readonly IAccessHandler _accessHandler;
    private readonly ILogger<AccessHandlerMediatorAdapter> _logger;

    public AccessHandlerMediatorAdapter(
        IAccessHandler accessHandler,
        ILogger<AccessHandlerMediatorAdapter> logger)
    {
        _accessHandler = accessHandler;
        _logger = logger;
    }

    public async Task<(bool ShouldReturn, string Acesso)> Handle(
        HandleAccessCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adaptando IAccessHandler para MediatR - Handler: {HandlerType}",
            _accessHandler.GetType().Name);

        return await _accessHandler.HandleAccessAsync(request.Veiculo, request.Alphadigi);
    }
}