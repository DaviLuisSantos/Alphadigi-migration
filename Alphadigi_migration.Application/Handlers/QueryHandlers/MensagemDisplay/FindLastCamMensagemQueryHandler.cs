using Alphadigi_migration.Application.Queries.MensagemDisplay;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.MensagemDisplay;

public class FindLastCamMensagemQueryHandler : IRequestHandler<FindLastCamMensagemQuery, 
                                                               Domain.EntitiesNew.MensagemDisplay>
{
    private readonly IMensagemDisplayRepository _repository;
    private readonly ILogger<FindLastCamMensagemQueryHandler> _logger;

    public FindLastCamMensagemQueryHandler(
        IMensagemDisplayRepository repository,
        ILogger<FindLastCamMensagemQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Domain.EntitiesNew.MensagemDisplay> Handle(FindLastCamMensagemQuery request, 
                                                                 CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando última mensagem da câmera: {AlphadigiId}", request.AlphadigiId);

        // Converter Guid para int se necessário
        var alphadigiIdInt = request.AlphadigiId;

        var mensagemAntiga = await _repository.FindLastCamMensagemAsync(alphadigiIdInt);

        if (mensagemAntiga == null)
            return null;

        // Converter para nova entidade
        return new Domain.EntitiesNew.MensagemDisplay(
            mensagemAntiga.Placa,
            mensagemAntiga.Mensagem,
            request.AlphadigiId, // Usar o Guid original
            mensagemAntiga.DataHora);
    }

    private int ConvertGuidToInt(Guid guid)
    {
        // Implementação de conversão GUID para int
        byte[] bytes = guid.ToByteArray();
        return BitConverter.ToInt32(bytes, 0);
    }
}
