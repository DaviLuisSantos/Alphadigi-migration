using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class DeleteAlphadigiCommandHandler : IRequestHandler<DeleteAlphadigiCommand, bool>
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<DeleteAlphadigiCommandHandler> _logger;

    public DeleteAlphadigiCommandHandler(IAlphadigiRepository repository, ILogger<DeleteAlphadigiCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAlphadigiCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.Delete(request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar Alphadigi");
            throw;
        }
    }
}