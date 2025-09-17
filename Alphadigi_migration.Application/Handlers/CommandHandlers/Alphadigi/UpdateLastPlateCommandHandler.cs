using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class UpdateLastPlateCommandHandler : IRequestHandler<UpdateLastPlateCommand, bool>
{
    private readonly IAlphadigiRepository _repository;

    private readonly ILogger<UpdateLastPlateCommandHandler> _logger;

    public UpdateLastPlateCommandHandler(IAlphadigiRepository repository, 
                                         ILogger<UpdateLastPlateCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateLastPlateCommand request, 
                                   CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.UpdateLastPlate(request.Camera, request.Plate, request.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar última placa");
            throw;
        }
    }
}

