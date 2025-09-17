using MediatR;

namespace Alphadigi_migration.Application.Queries.MensagemDisplay;

public class FindLastCamMensagemQuery : IRequest<Domain.EntitiesNew.MensagemDisplay>
{
    public int AlphadigiId { get; set; }
}
