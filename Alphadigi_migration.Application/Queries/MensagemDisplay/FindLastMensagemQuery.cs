using MediatR;

namespace Alphadigi_migration.Application.Queries.MensagemDisplay;

public class FindLastMensagemQuery : IRequest<Domain.EntitiesNew.MensagemDisplay>
{
    public required string Placa { get; init; }
    public required string Mensagem { get; init; }
    public required Guid AlphadigiId { get; init; }
}
