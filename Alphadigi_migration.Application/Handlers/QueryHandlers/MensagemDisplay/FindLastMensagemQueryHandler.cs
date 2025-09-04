using Alphadigi_migration.Application.Queries.MensagemDisplay;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.MensagemDisplay;

public class FindLastMensagemQueryHandler : IRequestHandler<FindLastMensagemQuery, Domain.EntitiesNew.MensagemDisplay>
{
    private readonly IMensagemDisplayRepository _repository;
    private readonly ILogger<FindLastMensagemQueryHandler> _logger;

    public FindLastMensagemQueryHandler(
        IMensagemDisplayRepository repository,
        ILogger<FindLastMensagemQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.MensagemDisplay> Handle(FindLastMensagemQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando última mensagem com termo: {Termo}", request.Termo);

        // Converter para o DTO antigo se necessário
        var termo = new Domain.DTOs.Display.FindLastMessage
        {
            Search = request.Termo
        };

        var mensagemAntiga = await _repository.FindLastMensagemAsync(termo);

        if (mensagemAntiga == null)
            return null;

        // Converter para nova entidade
        return new Domain.EntitiesNew.MensagemDisplay(
            mensagemAntiga.Placa,
            mensagemAntiga.Mensagem,
            Guid.Parse(mensagemAntiga.AlphadigiId.ToString()), // Converter int para Guid
            mensagemAntiga.DataHora);
    }
}
