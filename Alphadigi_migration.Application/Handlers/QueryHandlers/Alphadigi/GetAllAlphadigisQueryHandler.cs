using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Alphadigi;

public class GetAllAlphadigisQueryHandler : IRequestHandler<GetAllAlphadigisQuery, List<Domain.EntitiesNew.Alphadigi>>
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<GetAllAlphadigisQueryHandler> _logger;

    public GetAllAlphadigisQueryHandler(IAlphadigiRepository repository, 
                                        ILogger<GetAllAlphadigisQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Domain.EntitiesNew.Alphadigi>> Handle(GetAllAlphadigisQuery request, 
                                                           CancellationToken cancellationToken)
    {
        try
        {
            return await _repository.GetAll();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os Alphadigis");
            throw;
        }
    }
}
