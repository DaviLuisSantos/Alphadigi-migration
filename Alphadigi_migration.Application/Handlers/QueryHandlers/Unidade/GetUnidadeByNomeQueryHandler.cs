using Alphadigi_migration.Application.Queries.Unidade;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Unidade
{
    public class GetUnidadeByNomeQueryHandler : IRequestHandler<GetUnidadeByNomeQuery, Domain.EntitiesNew.Unidade>
    {
        private readonly IUnidadeRepository _unidadeRepository;
        private readonly ILogger<GetUnidadeByNomeQueryHandler> _logger;

        public GetUnidadeByNomeQueryHandler(IUnidadeRepository unidadeRepository, ILogger<GetUnidadeByNomeQueryHandler> logger)
        {
            _unidadeRepository = unidadeRepository;
            _logger = logger;
        }

        public async Task<Domain.EntitiesNew.Unidade> Handle(GetUnidadeByNomeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando unidade pelo nome: {Nome}", request.Nome);
            var unidade = await _unidadeRepository.GetUnidadeByNomeAsync(request.Nome);

            if (unidade == null)
                _logger.LogWarning("Unidade {Nome} não encontrada", request.Nome);

            return unidade;
        }
    }
}
