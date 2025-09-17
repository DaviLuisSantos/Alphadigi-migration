using Alphadigi_migration.Application.Queries.MensagemDisplay;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.QueryHandlers.MensagemDisplay;

public class FindLastMensagemQueryHandler : IRequestHandler<FindLastMensagemQuery, 
                                                            Domain.EntitiesNew.MensagemDisplay>
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

    public async Task<Domain.EntitiesNew.MensagemDisplay> Handle(FindLastMensagemQuery request, 
                                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando última mensagem para Alphadigi: {AlphadigiId}", request.AlphadigiId);

        // Converter para o DTO antigo se necessário

        var mensagem = await _repository.FindLastMensagemAsync(
                 request.Placa,
                 request.Mensagem,
                 request.AlphadigiId);

        if (mensagem == null)
        {
              _logger.LogInformation("Nenhuma mensagem encontrada para os critérios informados");
            return null;

        }
        // Converter para nova entidade
        return new Domain.EntitiesNew.MensagemDisplay(
            mensagem.Placa,
            mensagem.Mensagem,
           mensagem.AlphadigiId,// Converter int para Guid
            mensagem.DataHora);
    }
}
