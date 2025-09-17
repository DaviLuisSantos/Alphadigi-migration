using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class UpdateAlphadigiEntityCommandHandler : IRequestHandler<UpdateAlphadigiEntityCommand, 
                                                                   Domain.EntitiesNew.Alphadigi>
{

    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<UpdateAlphadigiEntityCommandHandler> _logger;

    public UpdateAlphadigiEntityCommandHandler(IAlphadigiRepository repository, 
                                               ILogger<UpdateAlphadigiEntityCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.Alphadigi> Handle(UpdateAlphadigiEntityCommand request, 
                                                           CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.Update(request.Alphadigi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar entidade Alphadigi");
            throw;
        }
    }
}
