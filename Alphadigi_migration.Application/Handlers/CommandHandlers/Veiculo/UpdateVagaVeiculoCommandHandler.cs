using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;

public class UpdateVagaVeiculoCommandHandler : IRequestHandler<UpdateVagaVeiculoCommand, bool>
{
    private readonly IVeiculoRepository _repository;
    private readonly ILogger<UpdateVagaVeiculoCommandHandler> _logger;

    public UpdateVagaVeiculoCommandHandler(
        IVeiculoRepository repository,
        ILogger<UpdateVagaVeiculoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateVagaVeiculoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UpdateVagaVeiculo chamado com id: {request.Id} e dentro: {request.Dentro}");
        return await _repository.UpdateVagaVeiculoAsync(request.Id, request.Dentro);
    }
}

