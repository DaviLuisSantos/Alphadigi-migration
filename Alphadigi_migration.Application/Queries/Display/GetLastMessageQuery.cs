using MediatR;


namespace Alphadigi_migration.Application.Queries.Display;

public class GetLastMessageQuery : IRequest<Domain.EntitiesNew.MensagemDisplay>
{
    public string Placa { get; set; }
    public string Acesso { get; set; }
    public int AlphadigiId { get; set; }

    public GetLastMessageQuery(string placa, string acesso, int alphadigiId)
    {
        Placa = placa;
        Acesso = acesso;
        AlphadigiId = alphadigiId;
    }
}
