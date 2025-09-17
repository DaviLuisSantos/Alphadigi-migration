using Alphadigi_migration.Application.Queries.Acesso;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class PrepareLocalStringQueryHandler : IRequestHandler<PrepareLocalStringQuery, string>
{
    private readonly ILogger<PrepareLocalStringQueryHandler> _logger;

    public PrepareLocalStringQueryHandler(ILogger<PrepareLocalStringQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<string> Handle(PrepareLocalStringQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var alphadigi = request.Alphadigi;

            if (alphadigi == null)
            {
                return Task.FromResult("Sem local");
            }

            string sentido = alphadigi.Sentido ? "ENTRADA" : "SAIDA";
            string areaNome = alphadigi.Area?.Nome ?? "Área não especificada";

            var localString = $"{areaNome} - {alphadigi.Nome} - {sentido}";

            _logger.LogInformation($"Local string preparado: {localString}");
            return Task.FromResult(localString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao preparar string do local");
            return Task.FromResult("Local não disponível");
        }
    }
}
    
