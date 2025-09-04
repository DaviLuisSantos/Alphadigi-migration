using MediatR;

namespace Alphadigi_migration.Application.Queries.Display;

public class GetLastCamMessageQuery : IRequest<Domain.EntitiesNew.MensagemDisplay>
{
    public Guid AlphadigiId { get; set; }

    public GetLastCamMessageQuery(Guid alphadigiId)
    {
        AlphadigiId = alphadigiId;
    }
}
