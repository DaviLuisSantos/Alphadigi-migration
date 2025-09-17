using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class UpdateStageCommandHandler : IRequestHandler<UpdateStageCommand, bool>
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<UpdateStageCommandHandler> _logger;

    public UpdateStageCommandHandler(IAlphadigiRepository repository, 
                                     ILogger<UpdateStageCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateStageCommand request, 
                                   CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.UpdateStage(request.Stage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estágio");
            throw;
        }
    }
}
