using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;

public class UpdateLastAccessCommandHandler : IRequestHandler<UpdateLastAccessCommand, bool>
{
    private readonly IVeiculoRepository _repository;
    private readonly ILogger<UpdateLastAccessCommandHandler> _logger;

    public UpdateLastAccessCommandHandler(IVeiculoRepository repository, 
                                          ILogger<UpdateLastAccessCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateLastAccessCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UpdateLastAccess chamado com id: {request.IdVeiculo}");

        var lastAccessDto = new Domain.DTOs.Veiculos.LastAcessUpdateVeiculoDTO
        {
            IdVeiculo = request.IdVeiculo,
            IpCamera = request.IpCamera,
            TimeAccess = request.DataHoraAcesso
        };

        return await _repository.UpdateLastAccessAsync(lastAccessDto);
    }
}

