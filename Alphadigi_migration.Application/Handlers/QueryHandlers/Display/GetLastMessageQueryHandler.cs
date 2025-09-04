using Alphadigi_migration.Application.Queries.Display;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Display;

public class GetLastMessageQueryHandler : IRequestHandler<GetLastMessageQuery, Domain.EntitiesNew.MensagemDisplay>
{
    private readonly IMensagemDisplayRepository _mensagemDisplayRepository;

    public GetLastMessageQueryHandler(IMensagemDisplayRepository mensagemDisplayRepository)
    {
        _mensagemDisplayRepository = mensagemDisplayRepository;
    }

    public async Task<Domain.EntitiesNew.MensagemDisplay> Handle(GetLastMessageQuery request, CancellationToken cancellationToken)
    {
        return await _mensagemDisplayRepository.FindLastMensagemAsync(
            request.Placa, request.Acesso, request.AlphadigiId);
    }
}