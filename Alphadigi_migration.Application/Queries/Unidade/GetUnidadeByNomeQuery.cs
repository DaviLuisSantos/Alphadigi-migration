using MediatR;

namespace Alphadigi_migration.Application.Queries.Unidade;

public record GetUnidadeByNomeQuery(string Nome) : IRequest<Domain.EntitiesNew.Unidade>;
