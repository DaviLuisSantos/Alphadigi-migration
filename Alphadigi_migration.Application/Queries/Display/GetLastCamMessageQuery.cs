using MediatR;

namespace Alphadigi_migration.Application.Queries.Display;

public class GetLastCamMessageQuery : IRequest<Domain.EntitiesNew.MensagemDisplay>
{
    public int AlphadigiId { get; set; }

    public GetLastCamMessageQuery(int alphadigiId)
    {
        AlphadigiId = alphadigiId;
    }
}
