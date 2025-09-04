using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Veiculo;

public class GetVeiculosQueryHandler : IRequestHandler<GetVeiculosQuery, List<Domain.EntitiesNew.Veiculo>>
{
    private readonly IVeiculoRepository _repository;
    private readonly ILogger<GetVeiculosQueryHandler> _logger;

    public GetVeiculosQueryHandler(IVeiculoRepository repository, 
                                    ILogger<GetVeiculosQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<List<Domain.EntitiesNew.Veiculo>> Handle(GetVeiculosQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
