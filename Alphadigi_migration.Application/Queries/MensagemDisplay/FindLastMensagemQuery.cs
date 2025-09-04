using MediatR;

namespace Alphadigi_migration.Application.Queries.MensagemDisplay;

public class FindLastMensagemQuery : IRequest<Domain.EntitiesNew.MensagemDisplay>
{
    public string Termo { get; set; }
}
